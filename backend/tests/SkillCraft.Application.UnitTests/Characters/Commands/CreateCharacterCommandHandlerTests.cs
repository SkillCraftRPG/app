using Bogus;
using Logitar.EventSourcing;
using Logitar.Portal.Contracts.Search;
using MediatR;
using Moq;
using SkillCraft.Application.Lineages;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts.Characters;
using SkillCraft.Contracts.Lineages;
using SkillCraft.Domain;
using SkillCraft.Domain.Characters;
using SkillCraft.Domain.Lineages;
using SkillCraft.Domain.Personalities;
using SkillCraft.Domain.Worlds;
using Attribute = SkillCraft.Contracts.Attribute;

namespace SkillCraft.Application.Characters.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class CreateCharacterCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;
  private readonly Faker _faker = new();

  private readonly Mock<ICharacterQuerier> _characterQuerier = new();
  private readonly Mock<ILineageQuerier> _lineageQuerier = new();
  private readonly Mock<ILineageRepository> _lineageRepository = new();
  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<IPersonalityRepository> _personalityRepository = new();
  private readonly Mock<ISender> _sender = new();

  private readonly CreateCharacterCommandHandler _handler;

  private readonly WorldMock _world = new();
  private readonly Lineage _species;
  private readonly Lineage _nation;
  private readonly Personality _personality;

  public CreateCharacterCommandHandlerTests()
  {
    _handler = new(
      _characterQuerier.Object,
      _lineageQuerier.Object,
      _lineageRepository.Object,
      _permissionService.Object,
      _personalityRepository.Object,
      _sender.Object);

    _species = new(_world.Id, parent: null, new Name("Humain"), _world.OwnerId);
    _lineageRepository.Setup(x => x.LoadAsync(_species.Id, _cancellationToken)).ReturnsAsync(_species);

    _nation = new(_world.Id, _species, new Name("Orrin"), _world.OwnerId);
    _lineageRepository.Setup(x => x.LoadAsync(_nation.Id, _cancellationToken)).ReturnsAsync(_nation);

    _personality = new(_world.Id, new Name("Courroucé"), _world.OwnerId)
    {
      Attribute = Attribute.Agility
    };
    _personality.Update(_world.OwnerId);
    _personalityRepository.Setup(x => x.LoadAsync(_personality.Id, _cancellationToken)).ReturnsAsync(_personality);
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
      Age = 30,
      PersonalityId = _personality.Id.ToGuid(),
      CustomizationIds = [Guid.NewGuid(), Guid.NewGuid()]
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
    _permissionService.Verify(x => x.EnsureCanPreviewAsync(
      command,
      It.Is<EntityMetadata>(y => y.WorldId == _nation.WorldId && y.Type == EntityType.Personality && y.Id == _personality.Id.ToGuid() && y.Size > 0),
      _cancellationToken), Times.Once);

    _lineageQuerier.Verify(x => x.SearchAsync(It.IsAny<WorldId>(), It.IsAny<SearchLineagesPayload>(), It.IsAny<CancellationToken>()), Times.Never);

    _sender.Verify(x => x.Send(It.Is<SaveCharacterCommand>(y => y.Character.WorldId == _world.Id
      && y.Character.Name.Value == payload.Name.Trim()
      && y.Character.Player != null && y.Character.Player.Value == payload.Player.Trim()
      && y.Character.LineageId == _nation.Id
      && y.Character.Height == payload.Height
      && y.Character.Weight == payload.Weight
      && y.Character.Age == payload.Age
      && y.Character.PersonalityId == _personality.Id), _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should throw AggregateNotFoundException when the lineage could not be found.")]
  public async Task It_should_throw_AggregateNotFoundException_when_the_lineage_could_not_be_found()
  {
    CreateCharacterPayload payload = new("Heracles Aetos")
    {
      LineageId = Guid.NewGuid(),
      Height = 1.84,
      Weight = 84.6,
      Age = 30,
      PersonalityId = Guid.NewGuid()
    };
    CreateCharacterCommand command = new(payload);
    command.Contextualize(_world);

    var exception = await Assert.ThrowsAsync<AggregateNotFoundException<Lineage>>(async () => await _handler.Handle(command, _cancellationToken));
    Assert.Equal(new AggregateId(payload.LineageId).Value, exception.Id);
    Assert.Equal("LineageId", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw AggregateNotFoundException when the personality could not be found.")]
  public async Task It_should_throw_AggregateNotFoundException_when_the_personality_could_not_be_found()
  {
    CreateCharacterPayload payload = new("Heracles Aetos")
    {
      LineageId = _nation.Id.ToGuid(),
      Height = 1.84,
      Weight = 84.6,
      Age = 30,
      PersonalityId = Guid.NewGuid()
    };
    CreateCharacterCommand command = new(payload);
    command.Contextualize(_world);

    var exception = await Assert.ThrowsAsync<AggregateNotFoundException<Personality>>(async () => await _handler.Handle(command, _cancellationToken));
    Assert.Equal(new AggregateId(payload.PersonalityId).Value, exception.Id);
    Assert.Equal("PersonalityId", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw InvalidCharacterLineageException when the lineage has nations.")]
  public async Task It_should_throw_InvalidCharacterLineageException_when_the_lineage_has_nations()
  {
    CreateCharacterPayload payload = new("Heracles Aetos")
    {
      LineageId = _species.Id.ToGuid(),
      Height = 1.84,
      Weight = 84.6,
      Age = 30,
      PersonalityId = Guid.NewGuid()
    };
    CreateCharacterCommand command = new(payload);
    command.Contextualize(_world);

    SearchResults<LineageModel> results = new([new LineageModel { Id = Guid.NewGuid() }]);
    _lineageQuerier.Setup(x => x.SearchAsync(_world.Id, It.Is<SearchLineagesPayload>(y => y.ParentId == payload.LineageId), _cancellationToken))
      .ReturnsAsync(results);

    var exception = await Assert.ThrowsAsync<InvalidCharacterLineageException>(async () => await _handler.Handle(command, _cancellationToken));
    Assert.Equal(payload.LineageId, exception.Id);
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
      Age = 30,
      PersonalityId = Guid.NewGuid(),
      CustomizationIds = [Guid.NewGuid()]
    };
    CreateCharacterCommand command = new(payload);

    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.Handle(command, _cancellationToken));
    Assert.Equal(2, exception.Errors.Count());

    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "LineageId" && (Guid?)e.AttemptedValue == payload.LineageId);
    Assert.Contains(exception.Errors, e => e.ErrorCode == "CustomizationsValidator" && e.PropertyName == "CustomizationIds");
  }
}
