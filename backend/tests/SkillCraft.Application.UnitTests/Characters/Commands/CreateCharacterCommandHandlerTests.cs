using Bogus;
using MediatR;
using Moq;
using SkillCraft.Application.Characters.Creation;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Characters;
using SkillCraft.Contracts.Customizations;
using SkillCraft.Domain;
using SkillCraft.Domain.Aspects;
using SkillCraft.Domain.Castes;
using SkillCraft.Domain.Characters;
using SkillCraft.Domain.Customizations;
using SkillCraft.Domain.Educations;
using SkillCraft.Domain.Items;
using SkillCraft.Domain.Items.Properties;
using SkillCraft.Domain.Languages;
using SkillCraft.Domain.Lineages;
using SkillCraft.Domain.Personalities;
using SkillCraft.Domain.Talents;
using Attribute = SkillCraft.Contracts.Attribute;

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
  private readonly Personality _personality;
  private readonly Customization[] _customizations;
  private readonly Aspect[] _aspects;
  private readonly BaseAttributes _baseAttributes = new(agility: 9, coordination: 9, intellect: 6, presence: 10, sensitivity: 7, spirit: 6, vigor: 10,
      best: Attribute.Agility, worst: Attribute.Sensitivity, mandatory: [Attribute.Agility, Attribute.Vigor],
      optional: [Attribute.Coordination, Attribute.Vigor], extra: [Attribute.Agility, Attribute.Vigor]);
  private readonly Caste _caste;
  private readonly Education _education;
  private readonly Language _language;
  private readonly Item _item;

  private readonly Talent _acrobatics;
  private readonly Talent _athletics;
  private readonly Talent _melee;
  private readonly Talent _resistance;

  public CreateCharacterCommandHandlerTests()
  {
    _handler = new(_characterQuerier.Object, _lineageRepository.Object, _permissionService.Object, _sender.Object);

    _species = new(_world.Id, parent: null, new Name("Humain"), _world.OwnerId)
    {
      Languages = new(ids: [], extra: 1, text: null)
    };
    _nation = new(_world.Id, _species, new Name("Orrin"), _world.OwnerId);
    _personality = new(_world.Id, new Name("Courroucé"), _world.OwnerId);
    _customizations =
    [
      new Customization(_world.Id, CustomizationType.Gift, new Name("Réflexes"), _world.OwnerId),
      new Customization(_world.Id, CustomizationType.Disability, new Name("Pauvreté"), _world.OwnerId)
    ];
    _aspects =
    [
      new Aspect(_world.Id, new Name("Farouche"), _world.OwnerId)
      {
        Skills = new Skills(Skill.Melee, Skill.Survival)
      },
      new Aspect(_world.Id, new Name("Gymnaste"), _world.OwnerId)
      {
        Skills = new Skills(Skill.Acrobatics, Skill.Athletics)
      }
    ];
    _caste = new(_world.Id, new Name("Milicien"), _world.OwnerId)
    {
      Skill = Skill.Melee
    };
    _education = new(_world.Id, new Name("Champs de bataille"), _world.OwnerId)
    {
      Skill = Skill.Resistance
    };
    _language = new(_world.Id, new Name("Celfique"), _world.OwnerId);
    _item = new(_world.Id, new Name("Denier"), new MoneyProperties(), _world.OwnerId);

    _acrobatics = new(_world.Id, tier: 0, new Name("Acrobaties"), _world.OwnerId)
    {
      Skill = Skill.Acrobatics
    };
    _acrobatics.Update(_world.OwnerId);
    _athletics = new(_world.Id, tier: 0, new Name("Athlétisme"), _world.OwnerId)
    {
      Skill = Skill.Athletics
    };
    _athletics.Update(_world.OwnerId);
    _melee = new(_world.Id, tier: 0, new Name("Mêlée"), _world.OwnerId)
    {
      Skill = Skill.Melee
    };
    _melee.Update(_world.OwnerId);
    _resistance = new(_world.Id, tier: 0, new Name("Résistance"), _world.OwnerId)
    {
      Skill = Skill.Resistance
    };
    _resistance.Update(_world.OwnerId);

    _lineageRepository.Setup(x => x.LoadAsync(_species.Id, _cancellationToken)).ReturnsAsync(_species);
  }

  [Fact(DisplayName = "It should create a new character.")]
  public async Task It_should_create_a_new_character()
  {
    CreateCharacterPayload payload = new("  Heracles Aetos  ")
    {
      Player = $"  {_faker.Person.FullName}  ",
      LineageId = _nation.EntityId,
      Height = 1.84,
      Weight = 84.6,
      Age = 30,
      LanguageIds = [_language.EntityId],
      PersonalityId = _personality.EntityId,
      CustomizationIds = _customizations.Select(x => x.EntityId).ToList(),
      AspectIds = _aspects.Select(x => x.EntityId).ToList(),
      Attributes = new()
      {
        Agility = _baseAttributes.Agility,
        Coordination = _baseAttributes.Coordination,
        Intellect = _baseAttributes.Intellect,
        Presence = _baseAttributes.Presence,
        Sensitivity = _baseAttributes.Sensitivity,
        Spirit = _baseAttributes.Spirit,
        Vigor = _baseAttributes.Vigor,
        Best = _baseAttributes.Best,
        Worst = _baseAttributes.Worst,
        Optional = [.. _baseAttributes.Optional],
        Extra = [.. _baseAttributes.Extra]
      },
      CasteId = _caste.EntityId,
      EducationId = _education.EntityId,
      TalentIds = [_acrobatics.EntityId, _athletics.EntityId],
      StartingWealth = new StartingWealthPayload
      {
        ItemId = _item.EntityId,
        Quantity = 100
      }
    };
    CreateCharacterCommand command = new(payload);
    command.Contextualize(_world);

    _sender.Setup(x => x.Send(It.Is<ResolveLineageQuery>(y => y.Activity == command && y.Id == payload.LineageId), _cancellationToken)).ReturnsAsync(_nation);
    _sender.Setup(x => x.Send(It.Is<ResolvePersonalityQuery>(y => y.Activity == command && y.Id == payload.PersonalityId), _cancellationToken)).ReturnsAsync(_personality);
    _sender.Setup(x => x.Send(It.Is<ResolveCustomizationsQuery>(y => y.Activity == command
      && y.Personality == _personality && y.Ids == payload.CustomizationIds), _cancellationToken)).ReturnsAsync(_customizations);
    _sender.Setup(x => x.Send(It.Is<ResolveAspectsQuery>(y => y.Activity == command && y.Ids == payload.AspectIds), _cancellationToken)).ReturnsAsync(_aspects);
    _sender.Setup(x => x.Send(It.Is<ResolveBaseAttributesQuery>(y => y.Payload == payload.Attributes && y.Aspects == _aspects
      && y.Lineage == _nation && y.Parent == _species), _cancellationToken)).ReturnsAsync(_baseAttributes);
    _sender.Setup(x => x.Send(It.Is<ResolveCasteQuery>(y => y.Activity == command && y.Id == payload.CasteId), _cancellationToken)).ReturnsAsync(_caste);
    _sender.Setup(x => x.Send(It.Is<ResolveEducationQuery>(y => y.Activity == command && y.Id == payload.EducationId), _cancellationToken)).ReturnsAsync(_education);
    _sender.Setup(x => x.Send(It.Is<ResolveLanguagesQuery>(y => y.Activity == command && y.Lineage == _nation
      && y.Parent == _species && y.Ids == payload.LanguageIds), _cancellationToken)).ReturnsAsync([_language]);
    _sender.Setup(x => x.Send(It.Is<ResolveTalentsQuery>(y => y.Activity == command && y.Caste == _caste
      && y.Education == _education && y.Ids == payload.TalentIds), _cancellationToken)).ReturnsAsync([_acrobatics, _athletics, _melee, _resistance]);
    _sender.Setup(x => x.Send(It.Is<ResolveItemQuery>(y => y.Activity == command && y.Id == _item.EntityId), _cancellationToken)).ReturnsAsync(_item);

    CharacterModel character = new();
    _characterQuerier.Setup(x => x.ReadAsync(It.IsAny<Character>(), _cancellationToken)).ReturnsAsync(character);

    CharacterModel result = await _handler.Handle(command, _cancellationToken);
    Assert.Same(character, result);

    _permissionService.Verify(x => x.EnsureCanCreateAsync(command, EntityType.Character, _cancellationToken), Times.Once);

    _sender.Verify(x => x.Send(It.Is<SaveCharacterCommand>(y => y.Character.WorldId == _world.Id
      && y.Character.Name.Value == payload.Name.Trim()
      && y.Character.Player != null && y.Character.Player.Value == payload.Player.Trim()
      && y.Character.LineageId == _nation.Id
      && y.Character.Height == payload.Height
      && y.Character.Weight == payload.Weight
      && y.Character.Age == payload.Age
      && y.Character.PersonalityId == _personality.Id
      && y.Character.CustomizationIds.SequenceEqual(_customizations.Select(x => x.Id))
      && y.Character.AspectIds.SequenceEqual(_aspects.Select(x => x.Id))
      && y.Character.BaseAttributes == _baseAttributes
      && y.Character.CasteId == _caste.Id
      && y.Character.EducationId == _education.Id
      && HasLineageExtraLanguage(y.Character, _language)
      && HasTalent(y.Character, _acrobatics) && HasTalent(y.Character, _athletics) && HasTalent(y.Character, _melee) && HasTalent(y.Character, _resistance)
      && HasItem(y.Character, _item, payload.StartingWealth.Quantity)), _cancellationToken), Times.Once);
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
      AspectIds = [aspectId, aspectId],
      Attributes = new()
      {
        Agility = 12,
        Coordination = 10,
        Intellect = 10,
        Presence = 10,
        Sensitivity = 10,
        Spirit = 8,
        Vigor = 10,
        Best = Attribute.Agility,
        Worst = Attribute.Agility,
        Optional = [Attribute.Coordination, Attribute.Sensitivity, Attribute.Spirit, Attribute.Vigor],
        Extra = [(Attribute)(-1)]
      },
      CasteId = Guid.NewGuid(),
      EducationId = Guid.NewGuid(),
      TalentIds = [Guid.NewGuid()],
      StartingWealth = new StartingWealthPayload
      {
        ItemId = Guid.Empty,
        Quantity = -100
      }
    };
    CreateCharacterCommand command = new(payload);

    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.Handle(command, _cancellationToken));
    Assert.Equal(12, exception.Errors.Count());

    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "PersonalityId" && (Guid?)e.AttemptedValue == payload.PersonalityId);
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "CustomizationIds[0]");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "CreateCharacterValidator" && e.PropertyName == "AspectIds");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "InclusiveBetweenValidator" && e.PropertyName == "Attributes.Agility" && (int?)e.AttemptedValue == payload.Attributes.Agility);
    Assert.Contains(exception.Errors, e => e.ErrorCode == "BaseAttributeScoreSumValidator" && e.PropertyName == "Attributes");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEqualValidator" && e.PropertyName == "Attributes.Best" && (Attribute?)e.AttemptedValue == payload.Attributes.Best);
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEqualValidator" && e.PropertyName == "Attributes.Worst" && (Attribute?)e.AttemptedValue == payload.Attributes.Worst);
    Assert.Contains(exception.Errors, e => e.ErrorCode == "OptionalAttributesValidator" && e.PropertyName == "Attributes.Optional");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "EnumValidator" && e.PropertyName == "Attributes.Extra[0]" && (Attribute?)e.AttemptedValue == payload.Attributes.Extra.Single());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "CreateCharacterValidator" && e.PropertyName == "TalentIds");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "StartingWealth.ItemId");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "GreaterThanValidator" && e.PropertyName == "StartingWealth.Quantity");
  }

  private static bool HasItem(Character character, Item item, int quantity)
  {
    return character.Inventory.Values.Any(i => i.ItemId == item.Id && i.ContainingItemId == null && i.Quantity == quantity
      && i.IsAttuned == null && !i.IsEquipped && i.IsIdentified && i.IsProficient == null && i.Skill == null
      && i.RemainingCharges == null && i.RemainingResistance == null
      && i.NameOverride == null && i.DescriptionOverride == null && i.ValueOverride == null);
  }

  private static bool HasLineageExtraLanguage(Character character, Language language)
  {
    return character.Languages.TryGetValue(language.Id, out LanguageMetadata? metadata) && metadata.Notes?.Value == "Lineage Extra Language";
  }

  private bool HasTalent(Character character, Talent talent)
  {
    CharacterTalent? characterTalent = character.Talents.Values.SingleOrDefault(t => t.TalentId == talent.Id);
    if (characterTalent == null)
    {
      return false;
    }

    int expectedCost = talent.Tier + 2;
    IEnumerable<string> notes = characterTalent.Notes?.Value.Split("; ") ?? [];
    if (talent.Skill.HasValue)
    {
      if (talent.Skill == _caste.Skill && !notes.Contains($"Caste: {_caste.Name}"))
      {
        return false;
      }
      if (talent.Skill == _education.Skill && !notes.Contains($"Education: {_education.Name}"))
      {
        return false;
      }

      Dictionary<Skill, Aspect> discountedSkills = [];
      foreach (Aspect aspect in _aspects)
      {
        if (aspect.Skills.Discounted1.HasValue)
        {
          discountedSkills[aspect.Skills.Discounted1.Value] = aspect;
        }
        if (aspect.Skills.Discounted2.HasValue)
        {
          discountedSkills[aspect.Skills.Discounted2.Value] = aspect;
        }
      }
      if (discountedSkills.TryGetValue(talent.Skill.Value, out Aspect? discountedByAspect))
      {
        if (!notes.Contains($"Discounted by Aspect: {discountedByAspect.Name}"))
        {
          return false;
        }

        expectedCost--;
      }
    }

    return characterTalent.Cost == expectedCost;
  }
}
