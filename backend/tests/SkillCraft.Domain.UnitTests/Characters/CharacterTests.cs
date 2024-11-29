using SkillCraft.Contracts;
using SkillCraft.Contracts.Characters;
using SkillCraft.Contracts.Customizations;
using SkillCraft.Domain.Aspects;
using SkillCraft.Domain.Castes;
using SkillCraft.Domain.Customizations;
using SkillCraft.Domain.Educations;
using SkillCraft.Domain.Lineages;
using SkillCraft.Domain.Natures;
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
}
