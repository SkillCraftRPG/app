using SkillCraft.Contracts;
using SkillCraft.Contracts.Characters;
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
  private readonly Lineage _species;
  private readonly Lineage _nation;
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
    _species = new(_world.Id, parent: null, new Name("Humain"), _world.OwnerId)
    {
      Speeds = new Speeds(walk: 6, climb: 0, swim: 0, fly: 0, hover: 0, burrow: 0)
    };
    _species.Update(_world.OwnerId);
    _nation = new(_world.Id, _species, new Name("Orrin"), _world.OwnerId);
    _customization = new(_world.Id, CustomizationType.Gift, new Name("Féroce"), _world.OwnerId);
    _nature = new(_world.Id, new Name("Courroucé"), _world.OwnerId)
    {
      Attribute = Attribute.Agility
    };
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
      _species,
      _nation,
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

  [Fact(DisplayName = "AddBonus: it should add a new bonus.")]
  public void AddBonus_it_should_add_a_new_bonus()
  {
    Assert.Empty(_character.Bonuses);

    Bonus bonus = new(BonusCategory.Miscellaneous, MiscellaneousBonusTarget.Stamina.ToString(), value: 5, precision: new Name("Talent : Discipline"));
    _character.AddBonus(bonus, _world.OwnerId);

    KeyValuePair<Guid, Bonus> pair = Assert.Single(_character.Bonuses);
    Assert.NotEqual(default, pair.Key);
    Assert.Same(bonus, pair.Value);

    Assert.Contains(_character.Changes, c => c is Character.BonusUpdatedEvent e && e.BonusId != default && e.Bonus.Equals(bonus));
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

  [Fact(DisplayName = "AddTalent: it should throw ArgumentException when the talent resides in another world.")]
  public void AddTalent_it_should_throw_ArgumentException_when_the_talent_resides_in_another_world()
  {
    var exception = Assert.Throws<ArgumentException>(() => _character.AddTalent(_otherWorldTalent, _world.OwnerId));
    Assert.StartsWith("The talent does not reside in the same world as the character.", exception.Message);
    Assert.Equal("talent", exception.ParamName);
  }

  [Fact(DisplayName = "AddTalent: it should throw ArgumentOutOfRangeException when the cost is negative.")]
  public void AddTalent_it_should_throw_ArgumentOutOfRangeException_when_the_cost_is_negative()
  {
    SetTalentOptions options = new()
    {
      Cost = -_melee.MaximumCost
    };
    var exception = Assert.Throws<ArgumentOutOfRangeException>(() => _character.AddTalent(_melee, options, _world.OwnerId));
    Assert.StartsWith("The talent cost cannot be negative.", exception.Message);
    Assert.Equal("options", exception.ParamName);
  }

  [Fact(DisplayName = "AddTalent: it should throw NotEnoughRemainingTalentPointsException when there is not enough remaining talent points.")]
  public void AddTalent_it_should_throw_NotEnoughRemainingTalentPointsException_when_there_is_not_enough_remaining_talent_points()
  {
    _character.AddTalent(_melee, _world.OwnerId);
    _character.AddTalent(_formationMartiale, _world.OwnerId);
    _character.AddTalent(_occultisme, new SetTalentOptions
    {
      Cost = 1,
      Notes = new Description("Discounted by Aspect: Tenace")
    }, _world.OwnerId);
    _character.AddTalent(_elementarisme, new SetTalentOptions { Precision = new Name("Feu") }, _world.OwnerId);

    var exception = Assert.Throws<NotEnoughRemainingTalentPointsException>(() => _character.AddTalent(
      _elementarisme,
      new SetTalentOptions { Precision = new Name("Air") },
      _world.OwnerId));
    Assert.Equal(_world.Id.ToGuid(), exception.WorldId);
    Assert.Equal(_character.EntityId, exception.CharacterId);
    Assert.Equal(_character.RemainingTalentPoints, exception.RemainingTalentPoints);
    Assert.Equal(_elementarisme.EntityId, exception.TalentId);
    Assert.Equal(2, exception.Cost);
    Assert.Equal("TalentId", exception.PropertyName);
  }

  [Fact(DisplayName = "AddTalent: it should throw RequiredTalentNotPurchasedException when the required talent was not purchased first.")]
  public void AddTalent_it_should_throw_RequiredTalentNotPurchasedException_when_the_required_talent_was_not_purchased_first()
  {
    var exception = Assert.Throws<RequiredTalentNotPurchasedException>(() => _character.AddTalent(_formationMartiale, _world.OwnerId));
    Assert.Equal(_world.Id.ToGuid(), exception.WorldId);
    Assert.Equal(_character.EntityId, exception.CharacterId);
    Assert.Equal(_formationMartiale.EntityId, exception.RequiringTalentId);
    Assert.Equal(_melee.EntityId, exception.RequiredTalentId);
    Assert.Equal("TalentId", exception.PropertyName);
  }

  [Fact(DisplayName = "AddTalent: it should throw TalentCannotBePurchasedMultipleTimesException when the talent is being purchased multiple times.")]
  public void AddTalent_it_should_throw_TalentCannotBePurchasedMultipleTimesException_when_the_talent_is_being_purchased_multiple_times()
  {
    _character.AddTalent(_melee, _world.OwnerId);

    var exception = Assert.Throws<TalentCannotBePurchasedMultipleTimesException>(() => _character.AddTalent(_melee, _world.OwnerId));
    Assert.Equal(_world.Id.ToGuid(), exception.WorldId);
    Assert.Equal(_melee.EntityId, exception.TalentId);
    Assert.Equal("TalentId", exception.PropertyName);
  }

  [Fact(DisplayName = "AddTalent: it should throw TalentMaximumCostExceededException when the cost exceeds the maximum cost.")]
  public void AddTalent_it_should_throw_TalentMaximumCostExceededException_when_the_cost_exceeds_the_maximum_cost()
  {
    SetTalentOptions options = new()
    {
      Cost = _melee.MaximumCost + 1
    };
    var exception = Assert.Throws<TalentMaximumCostExceededException>(() => _character.AddTalent(_melee, options, _world.OwnerId));
    Assert.Equal(_world.Id.ToGuid(), exception.WorldId);
    Assert.Equal(_melee.EntityId, exception.TalentId);
    Assert.Equal(_melee.Tier, exception.Tier);
    Assert.Equal(_melee.MaximumCost, exception.MaximumCost);
    Assert.Equal(options.Cost, exception.AttemptedCost);
    Assert.Equal("Cost", exception.PropertyName);
  }

  [Fact(DisplayName = "AddTalent: it should throw TalentTierCannotExceedCharacterTierException when the talent tier is greater than the character tier.")]
  public void AddTalent_it_should_throw_TalentTierCannotExceedCharacterTierException_when_the_talent_tier_is_greater_than_the_character_tier()
  {
    var exception = Assert.Throws<TalentTierCannotExceedCharacterTierException>(() => _character.AddTalent(_cuirasse, _world.OwnerId));
    Assert.Equal(_world.Id.ToGuid(), exception.WorldId);
    Assert.Equal(_character.EntityId, exception.CharacterId);
    Assert.Equal(_character.Tier, exception.CharacterTier);
    Assert.Equal(_cuirasse.EntityId, exception.TalentId);
    Assert.Equal(_cuirasse.Tier, exception.TalentTier);
    Assert.Equal("TalentId", exception.PropertyName);
  }

  [Fact(DisplayName = "CanLevelUp: it should return false when the character cannot level-up yet.")]
  public void CanLevelUp_it_should_return_false_when_the_character_cannot_level_up_yet()
  {
    Assert.False(_character.CanLevelUp);

    _character.GainExperience(50, _world.OwnerId);
    Assert.False(_character.CanLevelUp);
  }

  [Fact(DisplayName = "CanLevelUp: it should return true when the character can level-up.")]
  public void CanLevelUp_it_should_return_true_when_the_character_can_level_up()
  {
    Assert.False(_character.CanLevelUp);

    _character.GainExperience(500, _world.OwnerId);
    Assert.True(_character.CanLevelUp);
  }

  [Fact(DisplayName = "CancelLevelUp: it should not do anything when there is no level-up.")]
  public void CancelLevelUp_it_should_not_do_anything_when_there_is_no_level_up()
  {
    _character.ClearChanges();
    Assert.False(_character.HasChanges);
    Assert.Empty(_character.Changes);

    _character.CancelLevelUp(_world.OwnerId);
    Assert.False(_character.HasChanges);
    Assert.Empty(_character.Changes);
  }

  [Fact(DisplayName = "CancelLevelUp: it should cancel the last level-up.")]
  public void CancelLevelUp_it_should_cancel_the_last_level_up()
  {
    _character.GainExperience(ExperienceTable.GetTotalExperience(_character.Level + 1), _world.OwnerId);
    Assert.True(_character.CanLevelUp);

    _character.LevelUp(Attribute.Agility, _world.OwnerId);
    _character.CancelLevelUp(_world.OwnerId);
    Assert.Empty(_character.LevelUps);
    Assert.Equal(0, _character.Level);

    Assert.Equal(16, _character.Attributes.Agility.Score);
  }

  [Theory(DisplayName = "GainExperience: it should increase the character experience by a positive number.")]
  [InlineData(30)]
  public void GainExperience_it_should_increase_the_character_experience_by_a_positive_number(int experience)
  {
    int previousExperience = _character.Experience;

    _character.GainExperience(experience, _world.OwnerId);
    Assert.Equal(previousExperience + experience, _character.Experience);
    Assert.Contains(_character.Changes, change => change is Character.ExperienceGainedEvent e && e.Experience == experience);
  }

  [Theory(DisplayName = "GainExperience: it should throw ArgumentOutOfRangeException when experience gain was zero or negative.")]
  [InlineData(0)]
  [InlineData(-25)]
  public void GainExperience_it_should_throw_ArgumentOutOfRangeException_when_experience_gain_was_zero_or_negative(int experience)
  {
    var exception = Assert.Throws<ArgumentOutOfRangeException>(() => _character.GainExperience(experience, _world.OwnerId));
    Assert.Equal("experience", exception.ParamName);
  }

  [Theory(DisplayName = "IncreaseSkillRank: it should increase the rank of the specified skill.")]
  [InlineData(Skill.Athletics)]
  public void IncreaseSkillRank_it_should_increase_the_rank_of_the_specified_skill(Skill skill)
  {
    _character.IncreaseSkillRank(skill, _world.OwnerId);
    Assert.Equal(1, _character.SkillRanks[skill]);
  }

  [Fact(DisplayName = "IncreaseSkillRank: it should throw ArgumentOutOfRangeException when the skill is not defined.")]
  public void IncreaseSkillRank_it_should_throw_ArgumentOutOfRangeException_when_the_skill_is_not_defined()
  {
    var exception = Assert.Throws<ArgumentOutOfRangeException>(() => _character.IncreaseSkillRank((Skill)(-1), _world.OwnerId));
    Assert.Equal("skill", exception.ParamName);
  }

  [Fact(DisplayName = "IncreaseSkillRank: it should throw NotEnoughRemainingSkillPointsException when the character has no remaining skill point.")]
  public void IncreaseSkillRank_it_should_throw_NotEnoughRemainingSkillPointsException_when_the_character_has_no_remaining_skill_point()
  {
    _character.IncreaseSkillRank(Skill.Acrobatics, _world.OwnerId);
    _character.IncreaseSkillRank(Skill.Acrobatics, _world.OwnerId);
    _character.IncreaseSkillRank(Skill.Athletics, _world.OwnerId);
    _character.IncreaseSkillRank(Skill.Athletics, _world.OwnerId);
    _character.IncreaseSkillRank(Skill.Resistance, _world.OwnerId);

    var exception = Assert.Throws<NotEnoughRemainingSkillPointsException>(() => _character.IncreaseSkillRank(Skill.Resistance, _world.OwnerId));
    Assert.Equal(_world.Id.ToGuid(), exception.WorldId);
    Assert.Equal(_character.EntityId, exception.CharacterId);
  }

  [Theory(DisplayName = "IncreaseSkillRank: it should throw SkillMaximumRankReachedException when the skill maximum rank has been reached.")]
  [InlineData(Skill.Athletics)]
  public void IncreaseSkillRank_it_should_throw_SkillMaximumRankReachedException_when_the_skill_maximum_rank_has_been_reached(Skill skill)
  {
    _character.IncreaseSkillRank(skill, _world.OwnerId);
    _character.IncreaseSkillRank(skill, _world.OwnerId);

    var exception = Assert.Throws<SkillMaximumRankReachedException>(() => _character.IncreaseSkillRank(skill, _world.OwnerId));
    Assert.Equal(_world.Id.ToGuid(), exception.WorldId);
    Assert.Equal(_character.EntityId, exception.CharacterId);
    Assert.Equal(_character.MaximumSkillRank, exception.MaximumSkillRank);
    Assert.Equal(Skill.Athletics, exception.Skill);
    Assert.Equal("Skill", exception.PropertyName);
  }

  [Fact(DisplayName = "It should account for correct skill points.")]
  public void It_should_account_for_correct_skill_points()
  {
    _character.AddBonus(new Bonus(BonusCategory.Statistic, Statistic.Learning.ToString(), value: +4, notes: new Description("Apprentissage accéléré")), _world.OwnerId);

    Assert.Equal(9, _character.AvailableSkillPoints);
    Assert.Equal(0, _character.SpentSkillPoints);
    Assert.Equal(9, _character.RemainingSkillPoints);

    _character.IncreaseSkillRank(Skill.Acrobatics, _world.OwnerId);
    _character.IncreaseSkillRank(Skill.Acrobatics, _world.OwnerId);
    _character.IncreaseSkillRank(Skill.Athletics, _world.OwnerId);
    _character.IncreaseSkillRank(Skill.Athletics, _world.OwnerId);
    _character.IncreaseSkillRank(Skill.Melee, _world.OwnerId);
    _character.IncreaseSkillRank(Skill.Melee, _world.OwnerId);
    _character.IncreaseSkillRank(Skill.Survival, _world.OwnerId);
    _character.IncreaseSkillRank(Skill.Survival, _world.OwnerId);

    Assert.Equal(9, _character.AvailableSkillPoints);
    Assert.Equal(8, _character.SpentSkillPoints);
    Assert.Equal(1, _character.RemainingSkillPoints);
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

  [Fact(DisplayName = "It should account for the correct attributes.")]
  public void It_should_account_for_the_correct_attributes()
  {
    _character.AddBonus(new Bonus(BonusCategory.Attribute, Attribute.Spirit.ToString(), value: -2), _world.OwnerId);
    _character.AddBonus(new Bonus(BonusCategory.Attribute, Attribute.Intellect.ToString(), value: +13, isTemporary: true), _world.OwnerId);

    _character.Experience = ExperienceTable.GetTotalExperience(_character.Level + 1);
    _character.Update(_world.OwnerId);

    _character.LevelUp(Attribute.Agility, _world.OwnerId);

    Assert.Equal(17, _character.Attributes.Agility.Score);
    Assert.Equal(3, _character.Attributes.Agility.Modifier);
    Assert.Equal(17, _character.Attributes.Agility.TemporaryScore);
    Assert.Equal(3, _character.Attributes.Agility.TemporaryModifier);

    Assert.Equal(10, _character.Attributes.Coordination.Score);
    Assert.Equal(0, _character.Attributes.Coordination.Modifier);
    Assert.Equal(10, _character.Attributes.Coordination.TemporaryScore);
    Assert.Equal(0, _character.Attributes.Coordination.TemporaryModifier);

    Assert.Equal(6, _character.Attributes.Intellect.Score);
    Assert.Equal(-2, _character.Attributes.Intellect.Modifier);
    Assert.Equal(19, _character.Attributes.Intellect.TemporaryScore);
    Assert.Equal(4, _character.Attributes.Intellect.TemporaryModifier);

    Assert.Equal(10, _character.Attributes.Presence.Score);
    Assert.Equal(0, _character.Attributes.Presence.Modifier);
    Assert.Equal(10, _character.Attributes.Presence.TemporaryScore);
    Assert.Equal(0, _character.Attributes.Presence.TemporaryModifier);

    Assert.Equal(8, _character.Attributes.Sensitivity.Score);
    Assert.Equal(-1, _character.Attributes.Sensitivity.Modifier);
    Assert.Equal(8, _character.Attributes.Sensitivity.TemporaryScore);
    Assert.Equal(-1, _character.Attributes.Sensitivity.TemporaryModifier);

    Assert.Equal(4, _character.Attributes.Spirit.Score);
    Assert.Equal(-3, _character.Attributes.Spirit.Modifier);
    Assert.Equal(4, _character.Attributes.Spirit.TemporaryScore);
    Assert.Equal(-3, _character.Attributes.Spirit.TemporaryModifier);

    Assert.Equal(14, _character.Attributes.Vigor.Score);
    Assert.Equal(2, _character.Attributes.Vigor.Modifier);
    Assert.Equal(14, _character.Attributes.Vigor.TemporaryScore);
    Assert.Equal(2, _character.Attributes.Vigor.TemporaryModifier);
  }

  [Fact(DisplayName = "It should account for the correct speeds.")]
  public void It_should_account_for_the_correct_speeds()
  {
    _character.AddBonus(new Bonus(BonusCategory.Speed, SpeedKind.Climb.ToString(), value: 3), _world.OwnerId);
    _character.AddBonus(new Bonus(BonusCategory.Speed, SpeedKind.Swim.ToString(), value: 3), _world.OwnerId);

    Assert.Equal(6, _character.Speeds.Walk);
    Assert.Equal(3, _character.Speeds.Climb);
    Assert.Equal(3, _character.Speeds.Swim);
    Assert.Equal(0, _character.Speeds.Fly);
    Assert.Equal(0, _character.Speeds.Hover);
    Assert.Equal(0, _character.Speeds.Burrow);
  }

  [Fact(DisplayName = "It should account for the correct statistics.")]
  public void It_should_account_for_the_correct_statistics()
  {
    _character.GainExperience(ExperienceTable.GetTotalExperience(_character.Level + 1), _world.OwnerId);
    Assert.True(_character.CanLevelUp);

    _character.LevelUp(Attribute.Agility, _world.OwnerId);

    _character.AddBonus(new Bonus(BonusCategory.Attribute, Attribute.Agility.ToString(), value: +2), _world.OwnerId);
    _character.AddBonus(new Bonus(BonusCategory.Statistic, Statistic.Learning.ToString(), value: +4), _world.OwnerId);

    Assert.Equal(42, _character.Statistics.Constitution.Value);
    Assert.Equal(7, _character.Statistics.Constitution.Increment);

    Assert.Equal(-1, _character.Statistics.Initiative.Value);
    Assert.Equal(0.2, _character.Statistics.Initiative.Increment);

    Assert.Equal(10, _character.Statistics.Learning.Value);
    Assert.Equal(1, _character.Statistics.Learning.Increment);

    Assert.Equal(-2, _character.Statistics.Power.Value);
    Assert.Equal(0.15, _character.Statistics.Power.Increment);

    Assert.Equal(0, _character.Statistics.Precision.Value);
    Assert.Equal(0.25, _character.Statistics.Precision.Increment);

    Assert.Equal(0, _character.Statistics.Reputation.Value);
    Assert.Equal(0.5, _character.Statistics.Reputation.Increment);

    Assert.Equal(4, _character.Statistics.Strength.Value);
    Assert.Equal(0.475, _character.Statistics.Strength.Increment);
  }

  [Fact(DisplayName = "It should store the lineage attributes.")]
  public void It_should_store_the_lineage_attributes()
  {
    Assert.Equal(2, _character.LineageAttributes.Count);
    Assert.Equal(_species.Attributes, _character.LineageAttributes[_species.Id]);
    Assert.Equal(_nation.Attributes, _character.LineageAttributes[_nation.Id]);
  }

  [Fact(DisplayName = "It should store the lineage speeds.")]
  public void It_should_store_the_lineage_speeds()
  {
    Assert.Equal(2, _character.LineageSpeeds.Count);
    Assert.Equal(_species.Speeds, _character.LineageSpeeds[_species.Id]);
    Assert.Equal(_nation.Speeds, _character.LineageSpeeds[_nation.Id]);
  }

  [Fact(DisplayName = "It should store the nature attribute.")]
  public void It_should_store_the_nature_attribute()
  {
    Assert.True(_character.NatureAttribute.HasValue);
    Assert.Equal(_nature.Attribute, _character.NatureAttribute);
  }

  [Fact(DisplayName = "It should throw ArgumentException when a customization is the same as the nature's gift.")]
  public void It_should_throw_ArgumentException_when_a_customization_is_the_same_as_the_nature_s_gift()
  {
    Customization[] customizations = [_customization];
    var exception = Assert.Throws<ArgumentException>(() => new Character(_world.Id, new Name("Heracles Aetos"), player: null, _species, _nation,
      height: 1.84, weight: 84.6, age: 30, _nature, customizations, _aspects, _baseAttributes, _caste, _education, _world.OwnerId));
    Assert.StartsWith("The customizations cannot include the gift of the nature.", exception.Message);
    Assert.Equal("customizations", exception.ParamName);
  }

  [Fact(DisplayName = "It should throw ArgumentException when a customization resides in another world.")]
  public void It_should_throw_ArgumentException_when_a_customization_resides_in_another_world()
  {
    Customization[] customizations = [new(WorldId.NewId(), CustomizationType.Gift, new Name("Féroce"), UserId.NewId())];
    var exception = Assert.Throws<ArgumentException>(() => new Character(_world.Id, new Name("Heracles Aetos"), player: null, _species, _nation,
      height: 1.84, weight: 84.6, age: 30, _nature, customizations, _aspects, _baseAttributes, _caste, _education, _world.OwnerId));
    Assert.StartsWith("One or more customizations do not reside in the same world as the character.", exception.Message);
    Assert.Equal("customizations", exception.ParamName);
  }

  [Fact(DisplayName = "It should throw ArgumentException when an aspect resides in another world.")]
  public void It_should_throw_ArgumentException_when_an_aspect_resides_in_another_world()
  {
    Aspect[] aspects = [_aspects[0], new(WorldId.NewId(), new Name("Gymnaste"), UserId.NewId())];
    var exception = Assert.Throws<ArgumentException>(() => new Character(_world.Id, new Name("Heracles Aetos"), player: null, _species, _nation,
      height: 1.84, weight: 84.6, age: 30, _nature, customizations: [], aspects, _baseAttributes, _caste, _education, _world.OwnerId));
    Assert.StartsWith("One or more aspects do not reside in the same world as the character.", exception.Message);
    Assert.Equal("aspects", exception.ParamName);
  }

  [Fact(DisplayName = "It should throw ArgumentException when not exactly two different aspects were provided.")]
  public void It_should_throw_ArgumentException_when_not_exactly_two_different_aspects_were_provided()
  {
    Aspect[] aspects = [_aspects[0], _aspects[0]];
    var exception = Assert.Throws<ArgumentException>(() => new Character(_world.Id, new Name("Heracles Aetos"), player: null, _species, _nation,
      height: 1.84, weight: 84.6, age: 30, _nature, customizations: [], aspects, _baseAttributes, _caste, _education, _world.OwnerId));
    Assert.StartsWith("Exactly 2 different aspects should be provided.", exception.Message);
    Assert.Equal("aspects", exception.ParamName);
  }

  [Fact(DisplayName = "It should throw ArgumentException when the caste resides in another world.")]
  public void It_should_throw_ArgumentException_when_the_caste_resides_in_another_world()
  {
    Caste caste = new(WorldId.NewId(), new Name("Amuseur"), UserId.NewId());

    var exception = Assert.Throws<ArgumentException>(() => new Character(_world.Id, new Name("Heracles Aetos"), player: null, _species, _nation,
      height: 1.84, weight: 84.6, age: 30, _nature, customizations: [], _aspects, _baseAttributes, caste, _education, _world.OwnerId));
    Assert.StartsWith("The caste does not reside in the same world as the character.", exception.Message);
    Assert.Equal("caste", exception.ParamName);
  }

  [Fact(DisplayName = "It should throw ArgumentException when the education resides in another world.")]
  public void It_should_throw_ArgumentException_when_the_education_resides_in_another_world()
  {
    Education education = new(WorldId.NewId(), new Name("Rebelle"), UserId.NewId());

    var exception = Assert.Throws<ArgumentException>(() => new Character(_world.Id, new Name("Heracles Aetos"), player: null, _species, _nation,
      height: 1.84, weight: 84.6, age: 30, _nature, customizations: [], _aspects, _baseAttributes, _caste, education, _world.OwnerId));
    Assert.StartsWith("The education does not reside in the same world as the character.", exception.Message);
    Assert.Equal("education", exception.ParamName);
  }

  [Fact(DisplayName = "It should throw ArgumentException when the nation belongs to another species.")]
  public void It_should_throw_ArgumentException_when_the_nation_belongs_to_another_species()
  {
    Lineage species = new(_world.Id, parent: null, new Name("Elfe"), _world.OwnerId);
    Lineage nation = new(_world.Id, species, new Name("Haut-Elfe"), _world.OwnerId);

    var exception = Assert.Throws<ArgumentException>(() => new Character(_world.Id, new Name("Heracles Aetos"), player: null, _species, nation,
      height: 1.84, weight: 84.6, age: 30, _nature, customizations: [], _aspects, _baseAttributes, _caste, _education, _world.OwnerId));
    Assert.StartsWith("The nation must belong to the species.", exception.Message);
    Assert.Equal("nation", exception.ParamName);
  }

  [Fact(DisplayName = "It should throw ArgumentException when the number of gifts does not equal the number of disabilities.")]
  public void It_should_throw_ArgumentException_when_the_number_of_gifts_does_not_equal_the_number_of_disabilities()
  {

    Customization[] customizations = [new(_world.Id, CustomizationType.Gift, new Name("Réflexes"), _world.OwnerId)];
    var exception = Assert.Throws<ArgumentException>(() => new Character(_world.Id, new Name("Heracles Aetos"), player: null, _species, _nation,
      height: 1.84, weight: 84.6, age: 30, _nature, customizations, _aspects, _baseAttributes, _caste, _education, _world.OwnerId));
    Assert.StartsWith("The customizations must contain an equal number of gifts and disabilities.", exception.Message);
    Assert.Equal("customizations", exception.ParamName);
  }

  [Fact(DisplayName = "It should throw ArgumentException when the nature resides in another world.")]
  public void It_should_throw_ArgumentException_when_the_nature_resides_in_another_world()
  {
    Nature nature = new(WorldId.NewId(), new Name("Courroucé"), UserId.NewId());

    var exception = Assert.Throws<ArgumentException>(() => new Character(_world.Id, new Name("Heracles Aetos"), player: null, _species, _nation,
      height: 1.84, weight: 84.6, age: 30, nature, customizations: [], _aspects, _baseAttributes, _caste, _education, _world.OwnerId));
    Assert.StartsWith("The nature does not reside in the same world as the character.", exception.Message);
    Assert.Equal("nature", exception.ParamName);
  }

  [Fact(DisplayName = "It should throw ArgumentException when the species is a nation.")]
  public void It_should_throw_ArgumentException_when_the_species_is_a_nation()
  {
    var exception = Assert.Throws<ArgumentException>(() => new Character(_world.Id, new Name("Heracles Aetos"), player: null, species: _nation, nation: null,
      height: 1.84, weight: 84.6, age: 30, _nature, customizations: [], _aspects, _baseAttributes, _caste, _education, _world.OwnerId));
    Assert.StartsWith("The species cannot be a nation.", exception.Message);
    Assert.Equal("species", exception.ParamName);
  }

  [Fact(DisplayName = "It should throw ArgumentException when the species resides in another world.")]
  public void It_should_throw_ArgumentException_when_the_species_resides_in_another_world()
  {
    Lineage species = new(WorldId.NewId(), parent: null, new Name("Elfe"), UserId.NewId());

    var exception = Assert.Throws<ArgumentException>(() => new Character(_world.Id, new Name("Heracles Aetos"), player: null, species, _nation,
      height: 1.84, weight: 84.6, age: 30, _nature, customizations: [], _aspects, _baseAttributes, _caste, _education, _world.OwnerId));
    Assert.StartsWith("The species does not reside in the same world as the character.", exception.Message);
    Assert.Equal("species", exception.ParamName);
  }

  [Fact(DisplayName = "It should throw ArgumentOutOfRangeException when setting a negative Blood Alcohol Content.")]
  public void It_should_throw_ArgumentOutOfRangeException_when_setting_a_negative_Blood_Alcohol_Content()
  {
    var exception = Assert.Throws<ArgumentOutOfRangeException>(() => _character.BloodAlcoholContent = -1);
    Assert.Equal("BloodAlcoholContent", exception.ParamName);
  }

  [Fact(DisplayName = "It should throw ArgumentOutOfRangeException when setting a negative Intoxication.")]
  public void It_should_throw_ArgumentOutOfRangeException_when_setting_a_negative_Intoxication()
  {
    var exception = Assert.Throws<ArgumentOutOfRangeException>(() => _character.Intoxication = -3);
    Assert.Equal("Intoxication", exception.ParamName);
  }

  [Fact(DisplayName = "It should throw ArgumentOutOfRangeException when setting a negative Stamina.")]
  public void It_should_throw_ArgumentOutOfRangeException_when_setting_a_negative_Stamina()
  {
    var exception = Assert.Throws<ArgumentOutOfRangeException>(() => _character.Stamina = -12);
    Assert.Equal("Stamina", exception.ParamName);
  }

  [Fact(DisplayName = "It should throw ArgumentOutOfRangeException when setting a negative Vitality.")]
  public void It_should_throw_ArgumentOutOfRangeException_when_setting_a_negative_Vitality()
  {
    var exception = Assert.Throws<ArgumentOutOfRangeException>(() => _character.Vitality = -5);
    Assert.Equal("Vitality", exception.ParamName);
  }

  [Fact(DisplayName = "It should throw ArgumentOutOfRangeException when setting the experience below current level total experience.")]
  public void It_should_throw_ArgumentOutOfRangeException_when_setting_the_experience_below_current_level_total_experience()
  {
    var exception = Assert.Throws<ArgumentOutOfRangeException>(() => _character.Experience = -100);
    Assert.Equal("Experience", exception.ParamName);
  }

  [Theory(DisplayName = "It should throw ArgumentOutOfRangeException when the age is not stricly positive.")]
  [InlineData(0)]
  [InlineData(-30)]
  public void It_should_throw_ArgumentOutOfRangeException_when_the_age_is_not_stricly_positive(int age)
  {
    var exception = Assert.Throws<ArgumentOutOfRangeException>(() => new Character(_world.Id, new Name("Heracles Aetos"), player: null, _species, _nation,
      height: 1.84, weight: 84.6, age, _nature, customizations: [], _aspects, _baseAttributes, _caste, _education, _world.OwnerId));
    Assert.Equal("age", exception.ParamName);

    exception = Assert.Throws<ArgumentOutOfRangeException>(() => _character.Age = age);
    Assert.Equal("Age", exception.ParamName);
  }

  [Theory(DisplayName = "It should throw ArgumentOutOfRangeException when the height is not stricly positive.")]
  [InlineData(0)]
  [InlineData(-1.84)]
  public void It_should_throw_ArgumentOutOfRangeException_when_the_height_is_not_stricly_positive(double height)
  {
    var exception = Assert.Throws<ArgumentOutOfRangeException>(() => new Character(_world.Id, new Name("Heracles Aetos"), player: null, _species, _nation,
      height, weight: 84.6, age: 30, _nature, customizations: [], _aspects, _baseAttributes, _caste, _education, _world.OwnerId));
    Assert.Equal("height", exception.ParamName);

    exception = Assert.Throws<ArgumentOutOfRangeException>(() => _character.Height = height);
    Assert.Equal("Height", exception.ParamName);
  }

  [Theory(DisplayName = "It should throw ArgumentOutOfRangeException when the weight is not stricly positive.")]
  [InlineData(0)]
  [InlineData(-84.6)]
  public void It_should_throw_ArgumentOutOfRangeException_when_the_weight_is_not_stricly_positive(double weight)
  {
    var exception = Assert.Throws<ArgumentOutOfRangeException>(() => new Character(_world.Id, new Name("Heracles Aetos"), player: null, _species, _nation,
      height: 1.84, weight, age: 30, _nature, customizations: [], _aspects, _baseAttributes, _caste, _education, _world.OwnerId));
    Assert.Equal("weight", exception.ParamName);

    exception = Assert.Throws<ArgumentOutOfRangeException>(() => _character.Weight = weight);
    Assert.Equal("Weight", exception.ParamName);
  }
  [Fact(DisplayName = "LevelUp: it should level-up the character.")]
  public void LevelUp_it_should_level_up_the_character()
  {
    _character.GainExperience(ExperienceTable.GetTotalExperience(_character.Level + 1), _world.OwnerId);
    Assert.True(_character.CanLevelUp);

    _character.LevelUp(Attribute.Agility, _world.OwnerId);
    Assert.Equal(1, _character.Level);

    LevelUp levelUp = Assert.Single(_character.LevelUps);
    Assert.Equal(Attribute.Agility, levelUp.Attribute);
    Assert.Equal(7, levelUp.Constitution);
    Assert.Equal(0.2, levelUp.Initiative);
    Assert.Equal(1, levelUp.Learning);
    Assert.Equal(0.15, levelUp.Power);
    Assert.Equal(0.25, levelUp.Precision);
    Assert.Equal(0.5, levelUp.Reputation);
    Assert.Equal(0.425, levelUp.Strength);

    Assert.Equal(17, _character.Attributes.Agility.Score);

    Assert.Equal(42, _character.Statistics.Constitution.Value);
    Assert.Equal(-1, _character.Statistics.Initiative.Value);
    Assert.Equal(6, _character.Statistics.Learning.Value);
    Assert.Equal(-2, _character.Statistics.Power.Value);
    Assert.Equal(0, _character.Statistics.Precision.Value);
    Assert.Equal(0, _character.Statistics.Reputation.Value);
    Assert.Equal(3, _character.Statistics.Strength.Value);
  }

  [Fact(DisplayName = "LevelUp: it should throw ArgumentOutOfRangeException when the attribute is not defined.")]
  public void LevelUp_it_should_throw_ArgumentOutOfRangeException_when_the_attribute_is_not_defined()
  {
    _character.GainExperience(ExperienceTable.GetTotalExperience(_character.Level + 1), _world.OwnerId);
    Assert.True(_character.CanLevelUp);

    var exception = Assert.Throws<ArgumentOutOfRangeException>(() => _character.LevelUp((Attribute)(-1), _world.OwnerId));
    Assert.Equal("attribute", exception.ParamName);
  }

  [Fact(DisplayName = "LevelUp: it should throw CharacterCannotLevelUpYetException when the character cannot level-up.")]
  public void LevelUp_it_should_throw_CharacterCannotLevelUpYetException_when_the_character_cannot_level_up()
  {
    _character.GainExperience(75, _world.OwnerId);
    Assert.True(_character.Experience < ExperienceTable.GetTotalExperience(_character.Level + 1));

    var exception = Assert.Throws<CharacterCannotLevelUpYetException>(() => _character.LevelUp(Attribute.Agility, _world.OwnerId));
    Assert.Equal(_character.WorldId.ToGuid(), exception.WorldId);
    Assert.Equal(_character.EntityId, exception.CharacterId);
    Assert.Equal(_character.Experience, exception.CurrentExperience);
    Assert.Equal(_character.Level, exception.CurrentLevel);
    Assert.Equal(ExperienceTable.GetTotalExperience(_character.Level + 1), exception.RequiredExperience);
  }

  [Fact(DisplayName = "LevelUp: it should throw AttributeMaximumScoreReachedException when the attribute score is already superior or equal to 20.")]
  public void LevelUp_it_should_throw_AttributeMaximumScoreReachedException_when_the_attribute_score_is_already_superior_or_equal_to_20()
  {
    _character.AddBonus(new Bonus(BonusCategory.Attribute, Attribute.Agility.ToString(), value: 4), _world.OwnerId);
    Assert.Equal(20, _character.Attributes.Agility.Score);

    _character.GainExperience(ExperienceTable.GetTotalExperience(_character.Level + 1), _world.OwnerId);
    Assert.True(_character.CanLevelUp);

    var exception = Assert.Throws<AttributeMaximumScoreReachedException>(() => _character.LevelUp(Attribute.Agility, _world.OwnerId));
    Assert.Equal(_world.Id.ToGuid(), exception.WorldId);
    Assert.Equal(_character.EntityId, exception.CharacterId);
    Assert.Equal(Attribute.Agility, exception.Attribute);
    Assert.Equal("Attribute", exception.PropertyName);
  }

  [Theory(DisplayName = "MaximumSkillRank: it should return the correct maximum rank.")]
  [InlineData(0, 2)]
  public void MaximumSkillRank_it_should_return_the_correct_maximum_rank(int tier, int maximumRank)
  {
    Assert.Equal(0, tier); // NOTE(fpion): reserved for future use.

    Assert.Equal(maximumRank, _character.MaximumSkillRank);
  }

  [Fact(DisplayName = "RemoveBonus: it should not do anything when the bonus was not found.")]
  public void RemoveBonus_it_should_not_do_anything_when_the_bonus_was_not_found()
  {
    Bonus bonus = new(BonusCategory.Speed, SpeedKind.Swim.ToString(), value: 6);
    _character.AddBonus(bonus, _world.OwnerId);

    _character.ClearChanges();
    Assert.False(_character.HasChanges);
    Assert.Empty(_character.Changes);

    _character.RemoveBonus(Guid.NewGuid(), _world.OwnerId);

    _character.ClearChanges();
    Assert.False(_character.HasChanges);
    Assert.Empty(_character.Changes);

    Assert.Same(bonus, Assert.Single(_character.Bonuses).Value);
  }

  [Fact(DisplayName = "RemoveBonus: it should remove an bonus language.")]
  public void RemoveBonus_it_should_remove_an_existing_bonus()
  {
    Guid bonusId = Guid.NewGuid();
    Bonus bonus = new(BonusCategory.Miscellaneous, MiscellaneousBonusTarget.Dodge.ToString(), value: +2);
    _character.SetBonus(bonusId, bonus, _world.OwnerId);
    Assert.NotEmpty(_character.Bonuses);

    _character.RemoveBonus(bonusId, _world.OwnerId);
    Assert.Empty(_character.Bonuses);

    Assert.Contains(_character.Changes, change => change is Character.BonusRemovedEvent e && e.BonusId == bonusId);
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

  [Fact(DisplayName = "RemoveTalent: it should not do anything when the talent was not found.")]
  public void RemoveTalent_it_should_not_do_anything_when_the_talent_was_not_found()
  {
    Assert.Empty(_character.Talents);
    _character.RemoveTalent(Guid.NewGuid(), _world.OwnerId);

    Assert.DoesNotContain(_character.Changes, change => change is Character.TalentRemovedEvent);
  }

  [Fact(DisplayName = "RemoveTalent: it should remove an existing talent.")]
  public void RemoveTalent_it_should_remove_an_existing_talent()
  {
    _character.AddTalent(_melee, _world.OwnerId);
    Guid id = Assert.Single(_character.Talents).Key;

    _character.RemoveTalent(id, _world.OwnerId);
    Assert.Empty(_character.Talents);

    Assert.Contains(_character.Changes, change => change is Character.TalentRemovedEvent e && e.RelationId == id);
  }

  [Fact(DisplayName = "SetBonus: it should not do anything when the bonus has not changed.")]
  public void SetBonus_it_should_not_do_anything_when_the_bonus_has_not_changed()
  {
    Assert.Empty(_character.Bonuses);

    Guid bonusId = Guid.NewGuid();
    Bonus bonus = new(BonusCategory.Statistic, Statistic.Learning.ToString(), value: +4);
    _character.SetBonus(bonusId, bonus, _world.OwnerId);

    _character.ClearChanges();
    Assert.False(_character.HasChanges);
    Assert.Empty(_character.Changes);

    _character.SetBonus(bonusId, bonus, _world.OwnerId);
    Assert.False(_character.HasChanges);
    Assert.Empty(_character.Changes);
  }

  [Fact(DisplayName = "SetBonus: it should replace an existing bonus.")]
  public void SetBonus_it_should_replace_an_existing_bonus()
  {
    Assert.Empty(_character.Bonuses);

    Guid bonusId = Guid.NewGuid();

    Bonus oldBonus = new(BonusCategory.Skill, Skill.Performance.ToString(), value: +1);
    _character.SetBonus(bonusId, oldBonus, _world.OwnerId);
    Assert.Contains(_character.Changes, c => c is Character.BonusUpdatedEvent e && e.BonusId == bonusId && e.Bonus.Equals(oldBonus));

    Bonus newBonus = new(BonusCategory.Skill, Skill.Perception.ToString(), value: +2, precision: new Name("Hat of Performance"));
    _character.SetBonus(bonusId, newBonus, _world.OwnerId);
    Assert.Contains(_character.Changes, c => c is Character.BonusUpdatedEvent e && e.BonusId == bonusId && e.Bonus.Equals(newBonus));

    KeyValuePair<Guid, Bonus> pair = Assert.Single(_character.Bonuses);
    Assert.Equal(bonusId, pair.Key);
    Assert.Same(newBonus, pair.Value);
  }

  [Fact(DisplayName = "SetBonus: it should set a new bonus.")]
  public void SetBonus_it_should_set_a_new_bonus()
  {
    Assert.Empty(_character.Bonuses);

    Guid bonusId = Guid.NewGuid();
    Bonus bonus = new(BonusCategory.Miscellaneous, MiscellaneousBonusTarget.Stamina.ToString(), value: 5, precision: new Name("Talent : Discipline"));
    _character.SetBonus(bonusId, bonus, _world.OwnerId);

    KeyValuePair<Guid, Bonus> pair = Assert.Single(_character.Bonuses);
    Assert.Equal(bonusId, pair.Key);
    Assert.Same(bonus, pair.Value);

    Assert.Contains(_character.Changes, c => c is Character.BonusUpdatedEvent e && e.BonusId == bonusId && e.Bonus.Equals(bonus));
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

  [Fact(DisplayName = "SetTalent: it should throw ArgumentException when the talent resides in another world.")]
  public void SetTalent_it_should_throw_ArgumentException_when_the_talent_resides_in_another_world()
  {
    var exception = Assert.Throws<ArgumentException>(() => _character.SetTalent(Guid.NewGuid(), _otherWorldTalent, _world.OwnerId));
    Assert.StartsWith("The talent does not reside in the same world as the character.", exception.Message);
    Assert.Equal("talent", exception.ParamName);
  }

  [Fact(DisplayName = "SetTalent: it should throw ArgumentOutOfRangeException when the cost is negative.")]
  public void SetTalent_it_should_throw_ArgumentOutOfRangeException_when_the_cost_is_negative()
  {
    SetTalentOptions options = new()
    {
      Cost = -_melee.MaximumCost
    };
    var exception = Assert.Throws<ArgumentOutOfRangeException>(() => _character.SetTalent(Guid.NewGuid(), _melee, options, _world.OwnerId));
    Assert.StartsWith("The talent cost cannot be negative.", exception.Message);
    Assert.Equal("options", exception.ParamName);
  }

  [Fact(DisplayName = "SetTalent: it should throw RequiredTalentNotPurchasedException when the required talent was not purchased first.")]
  public void SetTalent_it_should_throw_RequiredTalentNotPurchasedException_when_the_required_talent_was_not_purchased_first()
  {
    var exception = Assert.Throws<RequiredTalentNotPurchasedException>(() => _character.SetTalent(Guid.NewGuid(), _formationMartiale, _world.OwnerId));
    Assert.Equal(_world.Id.ToGuid(), exception.WorldId);
    Assert.Equal(_character.EntityId, exception.CharacterId);
    Assert.Equal(_formationMartiale.EntityId, exception.RequiringTalentId);
    Assert.Equal(_melee.EntityId, exception.RequiredTalentId);
    Assert.Equal("TalentId", exception.PropertyName);
  }

  [Fact(DisplayName = "SetTalent: it should throw NotEnoughRemainingTalentPointsException when there is not enough remaining talent points.")]
  public void SetTalent_it_should_throw_NotEnoughRemainingTalentPointsException_when_there_is_not_enough_remaining_talent_points()
  {
    _character.AddTalent(_melee, _world.OwnerId);
    _character.AddTalent(_formationMartiale, _world.OwnerId);
    _character.AddTalent(_occultisme, new SetTalentOptions
    {
      Cost = 1,
      Notes = new Description("Discounted by Aspect: Tenace")
    }, _world.OwnerId);
    _character.AddTalent(_elementarisme, new SetTalentOptions { Precision = new Name("Feu") }, _world.OwnerId);

    var exception = Assert.Throws<NotEnoughRemainingTalentPointsException>(() => _character.SetTalent(
      Guid.NewGuid(),
      _elementarisme,
      new SetTalentOptions { Precision = new Name("Air") },
      _world.OwnerId));
    Assert.Equal(_world.Id.ToGuid(), exception.WorldId);
    Assert.Equal(_character.EntityId, exception.CharacterId);
    Assert.Equal(_character.RemainingTalentPoints, exception.RemainingTalentPoints);
    Assert.Equal(_elementarisme.EntityId, exception.TalentId);
    Assert.Equal(2, exception.Cost);
    Assert.Equal("TalentId", exception.PropertyName);
  }

  [Fact(DisplayName = "SetTalent: it should throw TalentCannotBePurchasedMultipleTimesException when the talent is being purchased multiple times.")]
  public void SetTalent_it_should_throw_TalentCannotBePurchasedMultipleTimesException_when_the_talent_is_being_purchased_multiple_times()
  {
    _character.AddTalent(_melee, _world.OwnerId);

    var exception = Assert.Throws<TalentCannotBePurchasedMultipleTimesException>(() => _character.SetTalent(Guid.NewGuid(), _melee, _world.OwnerId));
    Assert.Equal(_world.Id.ToGuid(), exception.WorldId);
    Assert.Equal(_melee.EntityId, exception.TalentId);
    Assert.Equal("TalentId", exception.PropertyName);
  }

  [Fact(DisplayName = "SetTalent: it should throw TalentMaximumCostExceededException when the cost exceeds the maximum cost.")]
  public void SetTalent_it_should_throw_TalentMaximumCostExceededException_when_the_cost_exceeds_the_maximum_cost()
  {
    _character.AddTalent(_melee, _world.OwnerId);
    Guid id = Assert.Single(_character.Talents.Keys);

    SetTalentOptions options = new()
    {
      Cost = _melee.MaximumCost + 1
    };
    var exception = Assert.Throws<TalentMaximumCostExceededException>(() => _character.SetTalent(id, _melee, options, _world.OwnerId));
    Assert.Equal(_world.Id.ToGuid(), exception.WorldId);
    Assert.Equal(_melee.EntityId, exception.TalentId);
    Assert.Equal(_melee.Tier, exception.Tier);
    Assert.Equal(_melee.MaximumCost, exception.MaximumCost);
    Assert.Equal(options.Cost, exception.AttemptedCost);
    Assert.Equal("Cost", exception.PropertyName);
  }

  [Fact(DisplayName = "SetTalent: it should throw TalentTierCannotExceedCharacterTierException when the talent tier is greater than the character tier.")]
  public void SetTalent_it_should_throw_TalentTierCannotExceedCharacterTierException_when_the_talent_tier_is_greater_than_the_character_tier()
  {
    var exception = Assert.Throws<TalentTierCannotExceedCharacterTierException>(() => _character.SetTalent(Guid.NewGuid(), _cuirasse, _world.OwnerId));
    Assert.Equal(_world.Id.ToGuid(), exception.WorldId);
    Assert.Equal(_character.EntityId, exception.CharacterId);
    Assert.Equal(_character.Tier, exception.CharacterTier);
    Assert.Equal(_cuirasse.EntityId, exception.TalentId);
    Assert.Equal(_cuirasse.Tier, exception.TalentTier);
    Assert.Equal("TalentId", exception.PropertyName);
  }

  [Fact(DisplayName = "SetTalent: it should update an existing talent.")]
  public void SetTalent_it_should_update_an_existing_talent()
  {
    _character.AddTalent(_melee, _world.OwnerId);
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
