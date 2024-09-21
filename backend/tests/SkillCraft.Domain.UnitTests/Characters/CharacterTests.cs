using SkillCraft.Contracts.Customizations;
using SkillCraft.Domain.Aspects;
using SkillCraft.Domain.Customizations;
using SkillCraft.Domain.Lineages;
using SkillCraft.Domain.Personalities;
using SkillCraft.Domain.Worlds;
using Attribute = SkillCraft.Contracts.Attribute;

namespace SkillCraft.Domain.Characters;

[Trait(Traits.Category, Categories.Unit)]
public class CharacterTests
{
  private readonly World _world = new(new Slug("ungar"), UserId.NewId());
  private readonly Lineage _lineage;
  private readonly Customization _customization;
  private readonly Personality _personality;
  private readonly Aspect[] _aspects;
  private readonly BaseAttributes _baseAttributes = new(agility: 9, coordination: 9, intellect: 6, presence: 10, sensitivity: 7, spirit: 6, vigor: 10,
    best: Attribute.Agility, worst: Attribute.Sensitivity, mandatory: [Attribute.Agility, Attribute.Vigor], optional: [Attribute.Coordination, Attribute.Vigor],
    extra: [Attribute.Agility, Attribute.Vigor]);

  public CharacterTests()
  {
    _lineage = new(_world.Id, parent: null, new Name("Humain"), _world.OwnerId);
    _customization = new(_world.Id, CustomizationType.Gift, new Name("Féroce"), _world.OwnerId);
    _personality = new(_world.Id, new Name("Courroucé"), _world.OwnerId);
    _personality.SetGift(_customization);
    _personality.Update(_world.OwnerId);
    _aspects =
    [
      new(_world.Id, new Name("Farouche"), _world.OwnerId),
      new(_world.Id, new Name("Gymnaste"), _world.OwnerId)
    ];
  }

  [Fact(DisplayName = "It should throw ArgumentException when a customization is the same as the personality's gift.")]
  public void It_should_throw_ArgumentException_when_a_customization_is_the_same_as_the_personality_s_gift()
  {
    Customization[] customizations = [_customization];
    var exception = Assert.Throws<ArgumentException>(() => new Character(_world.Id, new Name("Heracles Aetos"), player: null, _lineage, height: 1.84,
      weight: 84.6, age: 30, _personality, customizations, _aspects, _baseAttributes, _world.OwnerId));
    Assert.StartsWith("The customizations cannot include the same gift as the personality.", exception.Message);
    Assert.Equal("customizations", exception.ParamName);
  }

  [Fact(DisplayName = "It should throw ArgumentException when a customization resides in another world.")]
  public void It_should_throw_ArgumentException_when_a_customization_resides_in_another_world()
  {
    Customization[] customizations = [new(WorldId.NewId(), CustomizationType.Gift, new Name("Féroce"), UserId.NewId())];
    var exception = Assert.Throws<ArgumentException>(() => new Character(_world.Id, new Name("Heracles Aetos"), player: null, _lineage, height: 1.84,
      weight: 84.6, age: 30, _personality, customizations, _aspects, _baseAttributes, _world.OwnerId));
    Assert.StartsWith("One or more customizations do not reside in the same world as the character.", exception.Message);
    Assert.Equal("customizations", exception.ParamName);
  }

  [Fact(DisplayName = "It should throw ArgumentException when an aspect resides in another world.")]
  public void It_should_throw_ArgumentException_when_an_aspect_resides_in_another_world()
  {
    Aspect[] aspects = [_aspects[0], new(WorldId.NewId(), new Name("Gymnaste"), UserId.NewId())];
    var exception = Assert.Throws<ArgumentException>(() => new Character(_world.Id, new Name("Heracles Aetos"), player: null, _lineage, height: 1.84,
      weight: 84.6, age: 30, _personality, customizations: [], aspects, _baseAttributes, _world.OwnerId));
    Assert.StartsWith("One or more aspects do not reside in the same world as the character.", exception.Message);
    Assert.Equal("aspects", exception.ParamName);
  }

  [Fact(DisplayName = "It should throw ArgumentException when not exactly two different aspects were provided.")]
  public void It_should_throw_ArgumentException_when_not_exactly_two_different_aspects_were_provided()
  {
    Aspect[] aspects = [_aspects[0], _aspects[0]];
    var exception = Assert.Throws<ArgumentException>(() => new Character(_world.Id, new Name("Heracles Aetos"), player: null, _lineage, height: 1.84,
      weight: 84.6, age: 30, _personality, customizations: [], aspects, _baseAttributes, _world.OwnerId));
    Assert.StartsWith("Exactly 2 different aspects should be provided.", exception.Message);
    Assert.Equal("aspects", exception.ParamName);
  }

  [Fact(DisplayName = "It should throw ArgumentException when the lineage resides in another world.")]
  public void It_should_throw_ArgumentException_when_the_lineage_resides_in_another_world()
  {
    Lineage lineage = new(WorldId.NewId(), parent: null, new Name("Elfe"), UserId.NewId());

    var exception = Assert.Throws<ArgumentException>(() => new Character(_world.Id, new Name("Heracles Aetos"), player: null, lineage, height: 1.84,
      weight: 84.6, age: 30, _personality, customizations: [], _aspects, _baseAttributes, _world.OwnerId));
    Assert.StartsWith("The lineage does not reside in the same world as the character.", exception.Message);
    Assert.Equal("lineage", exception.ParamName);
  }

  [Fact(DisplayName = "It should throw ArgumentException when the number of gifts does not equal the number of disabilities.")]
  public void It_should_throw_ArgumentException_when_the_number_of_gifts_does_not_equal_the_number_of_disabilities()
  {

    Customization[] customizations = [new(_world.Id, CustomizationType.Gift, new Name("Réflexes"), _world.OwnerId)];
    var exception = Assert.Throws<ArgumentException>(() => new Character(_world.Id, new Name("Heracles Aetos"), player: null, _lineage, height: 1.84,
      weight: 84.6, age: 30, _personality, customizations, _aspects, _baseAttributes, _world.OwnerId));
    Assert.StartsWith("The customizations must contain an equal number of gifts and disabilities.", exception.Message);
    Assert.Equal("customizations", exception.ParamName);
  }

  [Fact(DisplayName = "It should throw ArgumentException when the personality resides in another world.")]
  public void It_should_throw_ArgumentException_when_the_personality_resides_in_another_world()
  {
    Personality personality = new(WorldId.NewId(), new Name("Courroucé"), UserId.NewId());

    var exception = Assert.Throws<ArgumentException>(() => new Character(_world.Id, new Name("Heracles Aetos"), player: null, _lineage, height: 1.84,
      weight: 84.6, age: 30, personality, customizations: [], _aspects, _baseAttributes, _world.OwnerId));
    Assert.StartsWith("The personality does not reside in the same world as the character.", exception.Message);
    Assert.Equal("personality", exception.ParamName);
  }

  [Theory(DisplayName = "It should throw ArgumentOutOfRangeException when the age is not stricly positive.")]
  [InlineData(0)]
  [InlineData(-30)]
  public void It_should_throw_ArgumentOutOfRangeException_when_the_age_is_not_stricly_positive(int age)
  {
    var exception = Assert.Throws<ArgumentOutOfRangeException>(() => new Character(_world.Id, new Name("Heracles Aetos"), player: null, _lineage, height: 1.84,
      weight: 84.6, age, _personality, customizations: [], _aspects, _baseAttributes, _world.OwnerId));
    Assert.Equal("age", exception.ParamName);
  }

  [Theory(DisplayName = "It should throw ArgumentOutOfRangeException when the height is not stricly positive.")]
  [InlineData(0)]
  [InlineData(-1.84)]
  public void It_should_throw_ArgumentOutOfRangeException_when_the_height_is_not_stricly_positive(double height)
  {
    var exception = Assert.Throws<ArgumentOutOfRangeException>(() => new Character(_world.Id, new Name("Heracles Aetos"), player: null, _lineage, height,
      weight: 84.6, age: 30, _personality, customizations: [], _aspects, _baseAttributes, _world.OwnerId));
    Assert.Equal("height", exception.ParamName);
  }

  [Theory(DisplayName = "It should throw ArgumentOutOfRangeException when the weight is not stricly positive.")]
  [InlineData(0)]
  [InlineData(-84.6)]
  public void It_should_throw_ArgumentOutOfRangeException_when_the_weight_is_not_stricly_positive(double weight)
  {
    var exception = Assert.Throws<ArgumentOutOfRangeException>(() => new Character(_world.Id, new Name("Heracles Aetos"), player: null, _lineage, height: 1.84,
      weight, age: 30, _personality, customizations: [], _aspects, _baseAttributes, _world.OwnerId));
    Assert.Equal("weight", exception.ParamName);
  }
}
