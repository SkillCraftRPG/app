using Bogus;
using FluentValidation.Results;
using Logitar.EventSourcing;
using MediatR;
using Moq;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts.Characters;
using SkillCraft.Domain;
using SkillCraft.Domain.Characters;
using SkillCraft.Domain.Lineages;

namespace SkillCraft.Application.Characters.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class CreateCharacterCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;
  private readonly Faker _faker = new();

  private readonly Mock<ICharacterQuerier> _characterQuerier = new();
  private readonly Mock<ILineageRepository> _lineageRepository = new();
  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<ISender> _sender = new();

  private readonly CreateCharacterCommandHandler _handler;

  private readonly WorldMock _world = new();
  private readonly Lineage _species;
  private readonly Lineage _nation;

  public CreateCharacterCommandHandlerTests()
  {
    _handler = new(
      _characterQuerier.Object,
      _lineageRepository.Object,
      _permissionService.Object,
      _sender.Object);

    _species = new(_world.Id, parent: null, new Name("Humain"), _world.OwnerId);
    _lineageRepository.Setup(x => x.LoadAsync(_species.Id, _cancellationToken)).ReturnsAsync(_species);

    _nation = new(_world.Id, _species, new Name("Orrin"), _world.OwnerId);
    _lineageRepository.Setup(x => x.LoadAsync(_nation.Id, _cancellationToken)).ReturnsAsync(_nation);
  }

  [Fact(DisplayName = "It should create a new character.")]
  public async Task It_should_create_a_new_character()
  {
    CreateCharacterPayload payload = new("  Heracles Aetos  ")
    {
      Player = $"  {_faker.Person.FullName}  ",
      LineageId = _nation.Id.ToGuid(),
      Height = 1.84,
      Weight = 84.6,
      Age = 30
    };
    CreateCharacterCommand command = new(payload);
    command.Contextualize(_world);

    CharacterModel character = new();
    _characterQuerier.Setup(x => x.ReadAsync(It.IsAny<Character>(), _cancellationToken)).ReturnsAsync(character);

    CharacterModel result = await _handler.Handle(command, _cancellationToken);
    Assert.Same(character, result);

    _permissionService.Verify(x => x.EnsureCanCreateAsync(command, EntityType.Character, _cancellationToken), Times.Once);
    _permissionService.Verify(x => x.EnsureCanPreviewAsync(
      command,
      It.Is<EntityMetadata>(y => y.WorldId == _nation.WorldId && y.Type == EntityType.Lineage && y.Id == _nation.Id.ToGuid() && y.Size > 0),
      _cancellationToken), Times.Once);

    _sender.Verify(x => x.Send(It.Is<SaveCharacterCommand>(y => y.Character.WorldId == _world.Id
      && y.Character.Name.Value == payload.Name.Trim()
      && y.Character.Player != null && y.Character.Player.Value == payload.Player.Trim()
      && y.Character.LineageId == _nation.Id
      && y.Character.Height == payload.Height
      && y.Character.Weight == payload.Weight
      && y.Character.Age == payload.Age), _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should throw AggregateNotFoundException when the lineage could not be found.")]
  public async Task It_should_throw_AggregateNotFoundException_when_the_lineage_could_not_be_found()
  {
    CreateCharacterPayload payload = new("Heracles Aetos")
    {
      LineageId = Guid.NewGuid(),
      Height = 1.84,
      Weight = 84.6,
      Age = 30
    };
    CreateCharacterCommand command = new(payload);
    command.Contextualize(_world);

    var exception = await Assert.ThrowsAsync<AggregateNotFoundException<Lineage>>(async () => await _handler.Handle(command, _cancellationToken));
    Assert.Equal(new AggregateId(payload.LineageId).Value, exception.Id);
    Assert.Equal("LineageId", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    CreateCharacterPayload payload = new("Heracles Aetos")
    {
      LineageId = Guid.Empty,
      Height = 1.84,
      Weight = 84.6,
      Age = 30
    };
    CreateCharacterCommand command = new(payload);

    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.Handle(command, _cancellationToken));

    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("NotEmptyValidator", error.ErrorCode);
    Assert.Equal("LineageId", error.PropertyName);
    Assert.Equal(payload.LineageId, error.AttemptedValue);
  }
}
