using Bogus;
using MediatR;
using Moq;
using SkillCraft.Application.Characters.Creation;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts.Characters;
using SkillCraft.Contracts.Customizations;
using SkillCraft.Domain;
using SkillCraft.Domain.Aspects;
using SkillCraft.Domain.Characters;
using SkillCraft.Domain.Customizations;
using SkillCraft.Domain.Lineages;
using SkillCraft.Domain.Personalities;

namespace SkillCraft.Application.Characters.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class CreateCharacterCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;
  private readonly Faker _faker = new();

  private readonly Mock<ICharacterQuerier> _characterQuerier = new();
  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<ISender> _sender = new();

  private readonly CreateCharacterCommandHandler _handler;

  private readonly WorldMock _world = new();
  private readonly Lineage _lineage;
  private readonly Personality _personality;
  private readonly Customization[] _customizations;
  private readonly Aspect[] _aspects;

  public CreateCharacterCommandHandlerTests()
  {
    _handler = new(_characterQuerier.Object, _permissionService.Object, _sender.Object);

    _lineage = new(_world.Id, parent: null, new Name("Orrin"), _world.OwnerId);
    _personality = new(_world.Id, new Name("Courroucé"), _world.OwnerId);
    _customizations =
    [
      new Customization(_world.Id, CustomizationType.Gift, new Name("Réflexes"), _world.OwnerId),
      new Customization(_world.Id, CustomizationType.Disability, new Name("Pauvreté"), _world.OwnerId)
    ];
    _aspects =
    [
      new Aspect(_world.Id, new Name("Farouche"), _world.OwnerId),
      new Aspect(_world.Id, new Name("Gymnaste"), _world.OwnerId)
    ];
  }

  [Fact(DisplayName = "It should create a new character.")]
  public async Task It_should_create_a_new_character()
  {
    CreateCharacterPayload payload = new("  Heracles Aetos  ")
    {
      Player = $"  {_faker.Person.FullName}  ",
      LineageId = _lineage.Id.ToGuid(),
      Height = 1.84,
      Weight = 84.6,
      Age = 30,
      PersonalityId = _personality.Id.ToGuid(),
      CustomizationIds = _customizations.Select(x => x.Id.ToGuid()).ToList(),
      AspectIds = _aspects.Select(x => x.Id.ToGuid()).ToList()
    };
    CreateCharacterCommand command = new(payload);
    command.Contextualize(_world);

    _sender.Setup(x => x.Send(It.Is<ResolveLineageQuery>(y => y.Activity == command && y.Id == payload.LineageId), _cancellationToken)).ReturnsAsync(_lineage);
    _sender.Setup(x => x.Send(It.Is<ResolvePersonalityQuery>(y => y.Activity == command && y.Id == payload.PersonalityId), _cancellationToken)).ReturnsAsync(_personality);
    _sender.Setup(x => x.Send(It.Is<ResolveCustomizationsQuery>(y => y.Activity == command
      && y.Personality == _personality && y.Ids == payload.CustomizationIds), _cancellationToken)).ReturnsAsync(_customizations);

    CharacterModel character = new();
    _characterQuerier.Setup(x => x.ReadAsync(It.IsAny<Character>(), _cancellationToken)).ReturnsAsync(character);

    CharacterModel result = await _handler.Handle(command, _cancellationToken);
    Assert.Same(character, result);

    _permissionService.Verify(x => x.EnsureCanCreateAsync(command, EntityType.Character, _cancellationToken), Times.Once);

    _sender.Verify(x => x.Send(It.Is<SaveCharacterCommand>(y => y.Character.WorldId == _world.Id
      && y.Character.Name.Value == payload.Name.Trim()
      && y.Character.Player != null && y.Character.Player.Value == payload.Player.Trim()
      && y.Character.LineageId == _lineage.Id
      && y.Character.Height == payload.Height
      && y.Character.Weight == payload.Weight
      && y.Character.Age == payload.Age
      && y.Character.PersonalityId == _personality.Id
      && y.Character.CustomizationIds.SequenceEqual(_customizations.Select(x => x.Id))), _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    Guid aspectId = Guid.NewGuid();
    CreateCharacterPayload payload = new("Heracles Aetos")
    {
      LineageId = Guid.NewGuid(),
      Height = 1.84,
      Weight = 84.6,
      Age = 30,
      PersonalityId = Guid.Empty,
      CustomizationIds = [Guid.Empty],
      AspectIds = [aspectId, aspectId]
    };
    CreateCharacterCommand command = new(payload);

    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.Handle(command, _cancellationToken));
    Assert.Equal(3, exception.Errors.Count());

    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "PersonalityId" && (Guid?)e.AttemptedValue == payload.PersonalityId);
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "CustomizationIds[0]");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "CreateCharacterValidator" && e.PropertyName == "AspectIds");
  }
}
