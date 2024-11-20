using SkillCraft.Contracts;
using SkillCraft.Contracts.Customizations;
using SkillCraft.Domain.Aspects;
using SkillCraft.Domain.Castes;
using SkillCraft.Domain.Customizations;
using SkillCraft.Domain.Educations;
using SkillCraft.Domain.Items;
using SkillCraft.Domain.Items.Properties;
using SkillCraft.Domain.Languages;
using SkillCraft.Domain.Lineages;
using SkillCraft.Domain.Natures;
using SkillCraft.Domain.Talents;
using SkillCraft.Domain.Worlds;
using Attribute = SkillCraft.Contracts.Attribute;

namespace SkillCraft.Domain.Characters;

[Trait(Traits.Category, Categories.Unit)]
public class CharacterTests
{
  private readonly World _world = new(new Slug("ungar"), UserId.NewId());
  private readonly Lineage _lineage;
  private readonly Customization _customization;
  private readonly Nature _nature;
  private readonly Aspect[] _aspects;
  private readonly BaseAttributes _baseAttributes = new(agility: 9, coordination: 9, intellect: 6, presence: 10, sensitivity: 7, spirit: 6, vigor: 10,
    best: Attribute.Agility, worst: Attribute.Sensitivity, mandatory: [Attribute.Agility, Attribute.Vigor], optional: [Attribute.Coordination, Attribute.Vigor],
    extra: [Attribute.Agility, Attribute.Vigor]);
  private readonly Caste _caste;
  private readonly Education _education;
  private readonly Language _language;

  private readonly Talent _melee;
  private readonly Talent _otherWorldTalent;
  private readonly Talent _formationMartiale;
  private readonly Talent _cuirasse;
  private readonly Talent _occultisme;
  private readonly Talent _elementarisme;

  private readonly Item _commonClothes;
  private readonly Item _crystalOfDoom;
  private readonly Item _denier;
  private readonly Item _otherWorldItem;
  private readonly Item _pouch;

  private readonly Character _character;

  public CharacterTests()
  {
    _lineage = new(_world.Id, parent: null, new Name("Humain"), _world.OwnerId);
    _customization = new(_world.Id, CustomizationType.Gift, new Name("Féroce"), _world.OwnerId);
    _nature = new(_world.Id, new Name("Courroucé"), _world.OwnerId);
    _nature.SetGift(_customization);
    _nature.Update(_world.OwnerId);
    _aspects =
    [
      new(_world.Id, new Name("Farouche"), _world.OwnerId),
      new(_world.Id, new Name("Gymnaste"), _world.OwnerId)
    ];
    _caste = new(_world.Id, new Name("Milicien"), _world.OwnerId);
    _education = new(_world.Id, new Name("Champs de bataille"), _world.OwnerId);
    _language = new(_world.Id, new Name("Orrinique"), _world.OwnerId);

    _melee = new(_world.Id, tier: 0, new Name("Mêlée"), _world.OwnerId);
    _otherWorldTalent = new(WorldId.NewId(), tier: 0, new Name("Mêlée"), UserId.NewId());

    _formationMartiale = new(_world.Id, tier: 0, new Name("Formation martiale"), _world.OwnerId);
    _formationMartiale.SetRequiredTalent(_melee);
    _formationMartiale.Update(_world.OwnerId);

    _cuirasse = new(_world.Id, tier: 1, new Name("Cuirassé"), _world.OwnerId);
    _cuirasse.SetRequiredTalent(_formationMartiale);
    _cuirasse.Update(_world.OwnerId);

    _occultisme = new(_world.Id, tier: 0, new Name("Occultisme"), _world.OwnerId);
    _elementarisme = new(_world.Id, tier: 0, new Name("Élémentarisme"), _world.OwnerId)
    {
      AllowMultiplePurchases = true
    };
    _elementarisme.SetRequiredTalent(_occultisme);
    _elementarisme.Update(_world.OwnerId);

    _commonClothes = new(_world.Id, new Name("Vêtements communs"), new EquipmentProperties(defense: 0, resistance: null, traits: []), _world.OwnerId);
    _crystalOfDoom = new(_world.Id, new Name("Cristal de Damnation"), new MiscellaneousProperties(), _world.OwnerId);
    _denier = new(_world.Id, new Name("Denier"), new MoneyProperties(), _world.OwnerId)
    {
      Value = 1.0,
      Weight = 0.005
    };
    _denier.Update(_world.OwnerId);
    _otherWorldItem = new(WorldId.NewId(), new Name("Denier"), new MoneyProperties(), UserId.NewId());
    _pouch = new(_world.Id, new Name("Bourse"), new ContainerProperties(capacity: 2.5, volume: null), _world.OwnerId);

    _character = new(
      _world.Id,
      new Name("Heracles Aetos"),
      player: null,
      _lineage,
      height: 1.84,
      weight: 84.6,
      age: 30,
      _nature,
      customizations: [],
      _aspects,
      _baseAttributes,
      _caste,
      _education,
      _world.OwnerId);
  }

  [Fact(DisplayName = "AddItem: it should add a new item with options.")]
  public void AddItem_it_should_add_a_new_item_with_options()
  {
    SetItemOptions options = new()
    {
      Quantity = 2,
      IsIdentified = false,
      NameOverride = new Change<Name>(new Name("Cristal sombre")),
      DescriptionOverride = new Change<Description>(new Description("Un cristal opaque d’un noir profond."))
    };
    _character.AddItem(_crystalOfDoom, options, _world.OwnerId);

    CharacterItem item = Assert.Single(_character.Inventory.Values);
    Assert.Equal(_crystalOfDoom.Id, item.ItemId);
    Assert.Null(item.ContainingItemId);
    Assert.Equal(options.Quantity, item.Quantity);
    Assert.Null(item.IsAttuned);
    Assert.False(item.IsEquipped);
    Assert.Equal(options.IsIdentified, item.IsIdentified);
    Assert.Null(item.IsProficient);
    Assert.Null(item.Skill);
    Assert.Null(item.RemainingCharges);
    Assert.Null(item.RemainingResistance);
    Assert.Equal(options.NameOverride.Value, item.NameOverride);
    Assert.Equal(options.DescriptionOverride.Value, item.DescriptionOverride);
    Assert.Null(item.ValueOverride);
  }

  [Fact(DisplayName = "AddItem: it should add a new item without options.")]
  public void AddItem_it_should_add_a_new_item_without_options()
  {
    _character.AddItem(_commonClothes, _world.OwnerId);

    CharacterItem item = Assert.Single(_character.Inventory.Values);
    Assert.Equal(_commonClothes.Id, item.ItemId);
    Assert.Null(item.ContainingItemId);
    Assert.Equal(1, item.Quantity);
    Assert.Null(item.IsAttuned);
    Assert.False(item.IsEquipped);
    Assert.True(item.IsIdentified);
    Assert.Null(item.IsProficient);
    Assert.Null(item.Skill);
    Assert.Null(item.RemainingCharges);
    Assert.Null(item.RemainingResistance);
    Assert.Null(item.NameOverride);
    Assert.Null(item.DescriptionOverride);
    Assert.Null(item.ValueOverride);
  }

  [Fact(DisplayName = "AddItem: it should throw ArgumentException when the item resides in another world.")]
  public void AddItem_it_should_throw_ArgumentException_when_the_item_resides_in_another_world()
  {
    var exception = Assert.Throws<ArgumentException>(() => _character.AddItem(_otherWorldItem, _world.OwnerId));
    Assert.StartsWith("The item does not reside in the same world as the character.", exception.Message);
    Assert.Equal("item", exception.ParamName);
  }

  [Fact(DisplayName = "AddTalent: it should add a new talent with options.")]
  public void AddTalent_it_should_add_a_new_talent_with_options()
  {
    SetTalentOptions options = new()
    {
      Cost = 1,
      Precision = new Name("Melee"),
      Notes = new Description("Discounted by Aspect: Farouche")
    };
    _character.AddTalent(_melee, options, _world.OwnerId);

    CharacterTalent talent = Assert.Single(_character.Talents.Values);
    Assert.Equal(_melee.Id, talent.Id);
    Assert.Equal(options.Cost, talent.Cost);
    Assert.Equal(options.Precision, talent.Precision);
    Assert.Equal(options.Notes, talent.Notes);
  }

  [Fact(DisplayName = "AddTalent: it should add a new talent without options.")]
  public void AddTalent_it_should_add_a_new_talent_without_options()
  {
    _character.AddTalent(_melee, _world.OwnerId);

    CharacterTalent talent = Assert.Single(_character.Talents.Values);
    Assert.Equal(_melee.Id, talent.Id);
    Assert.Equal(_melee.Tier + 2, talent.Cost);
    Assert.Null(talent.Precision);
    Assert.Null(talent.Notes);
  }

  [Fact(DisplayName = "AddTalent: it should throw ArgumentException when the cost exceeds the maximum cost.")]
  public void AddTalent_it_should_throw_ArgumentException_when_the_cost_exceeds_the_maximum_cost()
  {
    SetTalentOptions options = new()
    {
      Cost = 3
    };
    var exception = Assert.Throws<ArgumentException>(() => _character.AddTalent(_melee, options, _world.OwnerId));
    Assert.StartsWith($"The cost cannot exceed the maximum cost (2) for the talent '{_melee}' of tier 0.", exception.Message);
    Assert.Equal("options", exception.ParamName);
  }

  [Fact(DisplayName = "AddTalent: it should throw ArgumentException when the talent resides in another world.")]
  public void AddTalent_it_should_throw_ArgumentException_when_the_talent_resides_in_another_world()
  {
    var exception = Assert.Throws<ArgumentException>(() => _character.AddTalent(_otherWorldTalent, _world.OwnerId));
    Assert.StartsWith("The talent does not reside in the same world as the character.", exception.Message);
    Assert.Equal("talent", exception.ParamName);
  }

  [Fact(DisplayName = "AddTalent: it should throw ArgumentException when the required talent was not purchased first.")]
  public void AddTalent_it_should_throw_ArgumentException_when_the_required_talent_was_not_purchased_first()
  {
    var exception = Assert.Throws<ArgumentException>(() => _character.AddTalent(_formationMartiale, _world.OwnerId));
    Assert.StartsWith("The character did not purchase the required talent yet.", exception.Message);
    Assert.Equal("talent", exception.ParamName);
  }

  [Fact(DisplayName = "AddTalent: it should throw ArgumentException when the talent is being purchased multiple times.")]
  public void AddTalent_it_should_throw_ArgumentException_when_the_talent_is_being_purchased_multiple_times()
  {
    _character.AddTalent(_melee, _world.OwnerId);

    var exception = Assert.Throws<ArgumentException>(() => _character.AddTalent(_melee, _world.OwnerId));
    Assert.StartsWith("The talent cannot be purchased multiple times.", exception.Message);
    Assert.Equal("talent", exception.ParamName);
  }

  [Fact(DisplayName = "AddTalent: it should throw ArgumentException when the talent tier is greater than the character tier.")]
  public void AddTalent_it_should_throw_ArgumentException_when_the_talent_tier_is_greater_than_the_character_tier()
  {
    var exception = Assert.Throws<ArgumentException>(() => _character.AddTalent(_cuirasse, _world.OwnerId));
    Assert.StartsWith("The talent tier (1) cannot exceed the character tier (0).", exception.Message);
    Assert.Equal("talent", exception.ParamName);
  }

  [Fact(DisplayName = "AddTalent: it should throw ArgumentException when there is not enough remaining talent points.")]
  public void AddTalent_it_should_throw_ArgumentException_when_there_is_not_enough_remaining_talent_points()
  {
    _character.AddTalent(_melee, _world.OwnerId);
    _character.AddTalent(_formationMartiale, _world.OwnerId);
    _character.AddTalent(_occultisme, new SetTalentOptions
    {
      Cost = 1,
      Notes = new Description("Discounted by Aspect: Tenace")
    }, _world.OwnerId);
    _character.AddTalent(_elementarisme, new SetTalentOptions { Precision = new Name("Feu") }, _world.OwnerId);

    var exception = Assert.Throws<ArgumentException>(() => _character.AddTalent(
      _elementarisme,
      new SetTalentOptions { Precision = new Name("Air") },
      _world.OwnerId));
    Assert.StartsWith("The cost (2) exceeds the remaining talent points (1).", exception.Message);
    Assert.Equal("talent", exception.ParamName);
  }

  [Fact(DisplayName = "It should account for correct talent points.")]
  public void It_should_account_for_correct_talent_points()
  {
    Assert.Equal(8, _character.AvailableTalentPoints);
    Assert.Equal(0, _character.SpentTalentPoints);
    Assert.Equal(8, _character.RemainingTalentPoints);

    _character.AddTalent(_melee, new SetTalentOptions { Cost = 1 }, _world.OwnerId);
    _character.SetTalent(Guid.NewGuid(), _formationMartiale, _world.OwnerId);
    _character.SetTalent(Guid.NewGuid(), _occultisme, new SetTalentOptions { Cost = 1 }, _world.OwnerId);
    _character.AddTalent(_elementarisme, _world.OwnerId);

    Assert.Equal(8, _character.AvailableTalentPoints);
    Assert.Equal(6, _character.SpentTalentPoints);
    Assert.Equal(2, _character.RemainingTalentPoints);
  }

  [Fact(DisplayName = "It should throw ArgumentException when a customization is the same as the nature's gift.")]
  public void It_should_throw_ArgumentException_when_a_customization_is_the_same_as_the_nature_s_gift()
  {
    Customization[] customizations = [_customization];
    var exception = Assert.Throws<ArgumentException>(() => new Character(_world.Id, new Name("Heracles Aetos"), player: null, _lineage, height: 1.84,
      weight: 84.6, age: 30, _nature, customizations, _aspects, _baseAttributes, _caste, _education, _world.OwnerId));
    Assert.StartsWith("The customizations cannot include the gift of the nature.", exception.Message);
    Assert.Equal("customizations", exception.ParamName);
  }

  [Fact(DisplayName = "It should throw ArgumentException when a customization resides in another world.")]
  public void It_should_throw_ArgumentException_when_a_customization_resides_in_another_world()
  {
    Customization[] customizations = [new(WorldId.NewId(), CustomizationType.Gift, new Name("Féroce"), UserId.NewId())];
    var exception = Assert.Throws<ArgumentException>(() => new Character(_world.Id, new Name("Heracles Aetos"), player: null, _lineage, height: 1.84,
      weight: 84.6, age: 30, _nature, customizations, _aspects, _baseAttributes, _caste, _education, _world.OwnerId));
    Assert.StartsWith("One or more customizations do not reside in the same world as the character.", exception.Message);
    Assert.Equal("customizations", exception.ParamName);
  }

  [Fact(DisplayName = "It should throw ArgumentException when an aspect resides in another world.")]
  public void It_should_throw_ArgumentException_when_an_aspect_resides_in_another_world()
  {
    Aspect[] aspects = [_aspects[0], new(WorldId.NewId(), new Name("Gymnaste"), UserId.NewId())];
    var exception = Assert.Throws<ArgumentException>(() => new Character(_world.Id, new Name("Heracles Aetos"), player: null, _lineage, height: 1.84,
      weight: 84.6, age: 30, _nature, customizations: [], aspects, _baseAttributes, _caste, _education, _world.OwnerId));
    Assert.StartsWith("One or more aspects do not reside in the same world as the character.", exception.Message);
    Assert.Equal("aspects", exception.ParamName);
  }

  [Fact(DisplayName = "It should throw ArgumentException when not exactly two different aspects were provided.")]
  public void It_should_throw_ArgumentException_when_not_exactly_two_different_aspects_were_provided()
  {
    Aspect[] aspects = [_aspects[0], _aspects[0]];
    var exception = Assert.Throws<ArgumentException>(() => new Character(_world.Id, new Name("Heracles Aetos"), player: null, _lineage, height: 1.84,
      weight: 84.6, age: 30, _nature, customizations: [], aspects, _baseAttributes, _caste, _education, _world.OwnerId));
    Assert.StartsWith("Exactly 2 different aspects should be provided.", exception.Message);
    Assert.Equal("aspects", exception.ParamName);
  }

  [Fact(DisplayName = "It should throw ArgumentException when the caste resides in another world.")]
  public void It_should_throw_ArgumentException_when_the_caste_resides_in_another_world()
  {
    Caste caste = new(WorldId.NewId(), new Name("Amuseur"), UserId.NewId());

    var exception = Assert.Throws<ArgumentException>(() => new Character(_world.Id, new Name("Heracles Aetos"), player: null, _lineage, height: 1.84,
      weight: 84.6, age: 30, _nature, customizations: [], _aspects, _baseAttributes, caste, _education, _world.OwnerId));
    Assert.StartsWith("The caste does not reside in the same world as the character.", exception.Message);
    Assert.Equal("caste", exception.ParamName);
  }

  [Fact(DisplayName = "It should throw ArgumentException when the education resides in another world.")]
  public void It_should_throw_ArgumentException_when_the_education_resides_in_another_world()
  {
    Education education = new(WorldId.NewId(), new Name("Rebelle"), UserId.NewId());

    var exception = Assert.Throws<ArgumentException>(() => new Character(_world.Id, new Name("Heracles Aetos"), player: null, _lineage, height: 1.84,
      weight: 84.6, age: 30, _nature, customizations: [], _aspects, _baseAttributes, _caste, education, _world.OwnerId));
    Assert.StartsWith("The education does not reside in the same world as the character.", exception.Message);
    Assert.Equal("education", exception.ParamName);
  }

  [Fact(DisplayName = "It should throw ArgumentException when the lineage resides in another world.")]
  public void It_should_throw_ArgumentException_when_the_lineage_resides_in_another_world()
  {
    Lineage lineage = new(WorldId.NewId(), parent: null, new Name("Elfe"), UserId.NewId());

    var exception = Assert.Throws<ArgumentException>(() => new Character(_world.Id, new Name("Heracles Aetos"), player: null, lineage, height: 1.84,
      weight: 84.6, age: 30, _nature, customizations: [], _aspects, _baseAttributes, _caste, _education, _world.OwnerId));
    Assert.StartsWith("The lineage does not reside in the same world as the character.", exception.Message);
    Assert.Equal("lineage", exception.ParamName);
  }

  [Fact(DisplayName = "It should throw ArgumentException when the number of gifts does not equal the number of disabilities.")]
  public void It_should_throw_ArgumentException_when_the_number_of_gifts_does_not_equal_the_number_of_disabilities()
  {

    Customization[] customizations = [new(_world.Id, CustomizationType.Gift, new Name("Réflexes"), _world.OwnerId)];
    var exception = Assert.Throws<ArgumentException>(() => new Character(_world.Id, new Name("Heracles Aetos"), player: null, _lineage, height: 1.84,
      weight: 84.6, age: 30, _nature, customizations, _aspects, _baseAttributes, _caste, _education, _world.OwnerId));
    Assert.StartsWith("The customizations must contain an equal number of gifts and disabilities.", exception.Message);
    Assert.Equal("customizations", exception.ParamName);
  }

  [Fact(DisplayName = "It should throw ArgumentException when the nature resides in another world.")]
  public void It_should_throw_ArgumentException_when_the_nature_resides_in_another_world()
  {
    Nature nature = new(WorldId.NewId(), new Name("Courroucé"), UserId.NewId());

    var exception = Assert.Throws<ArgumentException>(() => new Character(_world.Id, new Name("Heracles Aetos"), player: null, _lineage, height: 1.84,
      weight: 84.6, age: 30, nature, customizations: [], _aspects, _baseAttributes, _caste, _education, _world.OwnerId));
    Assert.StartsWith("The nature does not reside in the same world as the character.", exception.Message);
    Assert.Equal("nature", exception.ParamName);
  }

  [Theory(DisplayName = "It should throw ArgumentOutOfRangeException when the age is not stricly positive.")]
  [InlineData(0)]
  [InlineData(-30)]
  public void It_should_throw_ArgumentOutOfRangeException_when_the_age_is_not_stricly_positive(int age)
  {
    var exception = Assert.Throws<ArgumentOutOfRangeException>(() => new Character(_world.Id, new Name("Heracles Aetos"), player: null, _lineage, height: 1.84,
      weight: 84.6, age, _nature, customizations: [], _aspects, _baseAttributes, _caste, _education, _world.OwnerId));
    Assert.Equal("age", exception.ParamName);
  }

  [Theory(DisplayName = "It should throw ArgumentOutOfRangeException when the height is not stricly positive.")]
  [InlineData(0)]
  [InlineData(-1.84)]
  public void It_should_throw_ArgumentOutOfRangeException_when_the_height_is_not_stricly_positive(double height)
  {
    var exception = Assert.Throws<ArgumentOutOfRangeException>(() => new Character(_world.Id, new Name("Heracles Aetos"), player: null, _lineage, height,
      weight: 84.6, age: 30, _nature, customizations: [], _aspects, _baseAttributes, _caste, _education, _world.OwnerId));
    Assert.Equal("height", exception.ParamName);
  }

  [Theory(DisplayName = "It should throw ArgumentOutOfRangeException when the weight is not stricly positive.")]
  [InlineData(0)]
  [InlineData(-84.6)]
  public void It_should_throw_ArgumentOutOfRangeException_when_the_weight_is_not_stricly_positive(double weight)
  {
    var exception = Assert.Throws<ArgumentOutOfRangeException>(() => new Character(_world.Id, new Name("Heracles Aetos"), player: null, _lineage, height: 1.84,
      weight, age: 30, _nature, customizations: [], _aspects, _baseAttributes, _caste, _education, _world.OwnerId));
    Assert.Equal("weight", exception.ParamName);
  }

  [Fact(DisplayName = "RemoveLanguage: it should not do anything when the language was not found.")]
  public void RemoveLanguage_it_should_not_do_anything_when_the_language_was_not_found()
  {
    Assert.Empty(_character.Languages);
    _character.RemoveLanguage(_language.Id, _world.OwnerId);

    Assert.DoesNotContain(_character.Changes, change => change is Character.LanguageRemovedEvent);
  }

  [Fact(DisplayName = "RemoveLanguage: it should remove an existing language.")]
  public void RemoveLanguage_it_should_remove_an_existing_language()
  {
    _character.SetLanguage(_language, notes: null, _world.OwnerId);
    Assert.NotEmpty(_character.Languages);

    _character.RemoveLanguage(_language.Id, _world.OwnerId);
    Assert.Empty(_character.Languages);

    Assert.Contains(_character.Changes, change => change is Character.LanguageRemovedEvent e && e.LanguageId == _language.Id);
  }

  [Fact(DisplayName = "SetItem: it should add a new item.")]
  public void SetItem_it_should_add_a_new_item()
  {
    Guid id = Guid.NewGuid();
    _character.SetItem(id, _pouch, _world.OwnerId);

    Assert.Equal(id, Assert.Single(_character.Inventory.Keys));

    CharacterItem item = Assert.Single(_character.Inventory.Values);
    Assert.Equal(_pouch.Id, item.ItemId);
    Assert.Null(item.ContainingItemId);
    Assert.Equal(1, item.Quantity);
    Assert.Null(item.IsAttuned);
    Assert.False(item.IsEquipped);
    Assert.True(item.IsIdentified);
    Assert.Null(item.IsProficient);
    Assert.Null(item.Skill);
    Assert.Null(item.RemainingCharges);
    Assert.Null(item.RemainingResistance);
    Assert.Null(item.NameOverride);
    Assert.Null(item.DescriptionOverride);
    Assert.Null(item.ValueOverride);
  }

  [Fact(DisplayName = "SetItem: it should not do anything when the item did not change.")]
  public void SetItem_it_should_not_do_anything_when_the_item_did_not_change()
  {
    Guid id = Guid.NewGuid();
    _character.SetItem(id, _pouch, _world.OwnerId);
    _character.ClearChanges();

    _character.SetItem(id, _pouch, _world.OwnerId);
    Assert.Empty(_character.Changes);
    Assert.False(_character.HasChanges);
  }

  [Fact(DisplayName = "SetItem: it should throw ArgumentException when the item resides in another world.")]
  public void SetItem_it_should_throw_ArgumentException_when_the_item_resides_in_another_world()
  {
    var exception = Assert.Throws<ArgumentException>(() => _character.SetItem(Guid.NewGuid(), _otherWorldItem, _world.OwnerId));
    Assert.StartsWith("The item does not reside in the same world as the character.", exception.Message);
    Assert.Equal("item", exception.ParamName);
  }

  [Fact(DisplayName = "SetItem: it should update an existing item.")]
  public void SetItem_it_should_update_an_existing_item()
  {
    _character.AddItem(_pouch, _world.OwnerId);
    Guid pouchId = Assert.Single(_character.Inventory.Keys);

    Guid denierId = Guid.NewGuid();
    _character.SetItem(denierId, _denier, _world.OwnerId);

    SetItemOptions options = new()
    {
      ContainingItemId = new Change<Guid?>(pouchId),
      Quantity = 100
    };
    _character.SetItem(denierId, _denier, options, _world.OwnerId);

    Assert.Equal(2, _character.Inventory.Count);

    Assert.Contains(_character.Inventory, i => i.Key == pouchId && i.Value.ItemId == _pouch.Id);

    Assert.Contains(denierId, _character.Inventory.Keys);
    CharacterItem item = _character.Inventory[denierId];
    Assert.Equal(_denier.Id, item.ItemId);
    Assert.Equal(options.ContainingItemId.Value, item.ContainingItemId);
    Assert.Equal(options.Quantity, item.Quantity);
    Assert.Null(item.IsAttuned);
    Assert.False(item.IsEquipped);
    Assert.True(item.IsIdentified);
    Assert.Null(item.IsProficient);
    Assert.Null(item.Skill);
    Assert.Null(item.RemainingCharges);
    Assert.Null(item.RemainingResistance);
    Assert.Null(item.NameOverride);
    Assert.Null(item.DescriptionOverride);
    Assert.Null(item.ValueOverride);
  }

  [Fact(DisplayName = "SetLanguage: it should add a new language.")]
  public void SetLanguage_it_should_add_a_new_language()
  {
    _character.SetLanguage(_language, notes: null, _world.OwnerId);
    Assert.Contains(_character.Languages, x => x.Key == _language.Id && x.Value.Notes == null);
  }

  [Fact(DisplayName = "SetLanguage: it should not do anything when the language metadata did not change.")]
  public void SetLanguage_it_should_not_do_anything_when_the_language_metadata_did_not_change()
  {
    _character.SetLanguage(_language, notes: null, _world.OwnerId);
    _character.ClearChanges();

    _character.SetLanguage(_language, notes: null, _world.OwnerId);
    Assert.False(_character.HasChanges);
    Assert.Empty(_character.Changes);
  }

  [Fact(DisplayName = "SetLanguage: it should replace an existing language.")]
  public void SetLanguage_it_should_replace_an_existing_language()
  {
    _character.SetLanguage(_language, notes: null, _world.OwnerId);

    Description notes = new("Lineage Extra Language");
    _character.SetLanguage(_language, notes, _world.OwnerId);
    Assert.Contains(_character.Languages, x => x.Key == _language.Id && x.Value.Notes == notes);
  }

  [Fact(DisplayName = "SetLanguage: it should throw ArgumentException when the language resides in another world.")]
  public void SetLanguage_it_should_throw_ArgumentException_when_the_language_resides_in_another_world()
  {
    UserId userId = UserId.NewId();
    Language language = new(WorldId.NewId(), new Name("Orrinique"), userId);

    var exception = Assert.Throws<ArgumentException>(() => _character.SetLanguage(language, notes: null, userId));
    Assert.StartsWith("The language does not reside in the same world as the character.", exception.Message);
    Assert.Equal("language", exception.ParamName);
  }

  [Fact(DisplayName = "SetTalent: it should add a new talent.")]
  public void SetTalent_it_should_add_a_new_talent_without_options()
  {
    Guid id = Guid.NewGuid();
    _character.SetTalent(id, _melee, _world.OwnerId);

    Assert.Equal(id, Assert.Single(_character.Talents.Keys));

    CharacterTalent talent = Assert.Single(_character.Talents.Values);
    Assert.Equal(_melee.Id, talent.Id);
    Assert.Equal(_melee.Tier + 2, talent.Cost);
    Assert.Null(talent.Precision);
    Assert.Null(talent.Notes);
  }

  [Fact(DisplayName = "SetTalent: it should not do anything when the talent did not change.")]
  public void SetTalent_it_should_not_do_anything_when_the_talent_did_not_change()
  {
    _character.AddTalent(_occultisme, _world.OwnerId);

    Guid id = Guid.NewGuid();
    SetTalentOptions options = new()
    {
      Precision = new Name("Eau")
    };
    _character.SetTalent(id, _elementarisme, options, _world.OwnerId);
    _character.ClearChanges();

    _character.SetTalent(id, _elementarisme, options, _world.OwnerId);
    Assert.Empty(_character.Changes);
    Assert.False(_character.HasChanges);
  }

  [Fact(DisplayName = "SetTalent: it should throw ArgumentException when the cost exceeds the maximum cost.")]
  public void SetTalent_it_should_throw_ArgumentException_when_the_cost_exceeds_the_maximum_cost()
  {
    _character.AddTalent(_melee, _world.OwnerId);
    Guid id = Assert.Single(_character.Talents.Keys);

    SetTalentOptions options = new()
    {
      Cost = 3
    };
    var exception = Assert.Throws<ArgumentException>(() => _character.SetTalent(id, _melee, options, _world.OwnerId));
    Assert.StartsWith($"The cost cannot exceed the maximum cost (2) for the talent '{_melee}' of tier 0.", exception.Message);
    Assert.Equal("options", exception.ParamName);
  }

  [Fact(DisplayName = "SetTalent: it should throw ArgumentException when the talent resides in another world.")]
  public void SetTalent_it_should_throw_ArgumentException_when_the_talent_resides_in_another_world()
  {
    var exception = Assert.Throws<ArgumentException>(() => _character.SetTalent(Guid.NewGuid(), _otherWorldTalent, _world.OwnerId));
    Assert.StartsWith("The talent does not reside in the same world as the character.", exception.Message);
    Assert.Equal("talent", exception.ParamName);
  }

  [Fact(DisplayName = "SetTalent: it should throw ArgumentException when the required talent was not purchased first.")]
  public void SetTalent_it_should_throw_ArgumentException_when_the_required_talent_was_not_purchased_first()
  {
    var exception = Assert.Throws<ArgumentException>(() => _character.SetTalent(Guid.NewGuid(), _formationMartiale, _world.OwnerId));
    Assert.StartsWith("The character did not purchase the required talent yet.", exception.Message);
    Assert.Equal("talent", exception.ParamName);
  }

  [Fact(DisplayName = "SetTalent: it should throw ArgumentException when the talent is being purchased multiple times.")]
  public void SetTalent_it_should_throw_ArgumentException_when_the_talent_is_being_purchased_multiple_times()
  {
    _character.AddTalent(_melee, _world.OwnerId);

    var exception = Assert.Throws<ArgumentException>(() => _character.SetTalent(Guid.NewGuid(), _melee, _world.OwnerId));
    Assert.StartsWith("The talent cannot be purchased multiple times.", exception.Message);
    Assert.Equal("talent", exception.ParamName);
  }

  [Fact(DisplayName = "SetTalent: it should throw ArgumentException when the talent tier is greater than the character tier.")]
  public void SetTalent_it_should_throw_ArgumentException_when_the_talent_tier_is_greater_than_the_character_tier()
  {
    var exception = Assert.Throws<ArgumentException>(() => _character.SetTalent(Guid.NewGuid(), _cuirasse, _world.OwnerId));
    Assert.StartsWith("The talent tier (1) cannot exceed the character tier (0).", exception.Message);
    Assert.Equal("talent", exception.ParamName);
  }

  [Fact(DisplayName = "SetTalent: it should throw ArgumentException when there is not enough remaining talent points.")]
  public void SetTalent_it_should_throw_ArgumentException_when_there_is_not_enough_remaining_talent_points()
  {
    _character.AddTalent(_melee, _world.OwnerId);
    _character.AddTalent(_formationMartiale, _world.OwnerId);
    _character.AddTalent(_occultisme, new SetTalentOptions
    {
      Cost = 1,
      Notes = new Description("Discounted by Aspect: Tenace")
    }, _world.OwnerId);
    _character.AddTalent(_elementarisme, new SetTalentOptions { Precision = new Name("Feu") }, _world.OwnerId);

    var exception = Assert.Throws<ArgumentException>(() => _character.SetTalent(
      Guid.NewGuid(),
      _elementarisme,
      new SetTalentOptions { Precision = new Name("Air") },
      _world.OwnerId));
    Assert.StartsWith("The cost (2) exceeds the remaining talent points (1).", exception.Message);
    Assert.Equal("talent", exception.ParamName);
  }

  [Fact(DisplayName = "SetTalent: it should update an existing talent.")]
  public void SetTalent_it_should_update_an_existing_talent()
  {
    _character.AddTalent(_occultisme, _world.OwnerId);
    _character.AddTalent(_elementarisme, new SetTalentOptions { Precision = new Name("Terre") }, _world.OwnerId);

    Guid id = Guid.NewGuid();
    _character.SetTalent(id, _elementarisme, _world.OwnerId);

    SetTalentOptions options = new()
    {
      Precision = new Name("Esprit")
    };
    _character.SetTalent(id, _elementarisme, options, _world.OwnerId);

    CharacterTalent talent = _character.Talents[id];
    Assert.Equal(_elementarisme.Id, talent.Id);
    Assert.Equal(_elementarisme.Tier + 2, talent.Cost);
    Assert.Equal(options.Precision, talent.Precision);
    Assert.Equal(options.Notes, talent.Notes);
  }
}
