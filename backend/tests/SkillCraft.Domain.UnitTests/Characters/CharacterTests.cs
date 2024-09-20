using SkillCraft.Domain.Lineages;
using SkillCraft.Domain.Personalities;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Domain.Characters;

[Trait(Traits.Category, Categories.Unit)]
public class CharacterTests
{
  private readonly World _world = new(new Slug("ungar"), UserId.NewId());
  private readonly Lineage _lineage;
  private readonly Personality _personality;

  public CharacterTests()
  {
    _lineage = new(_world.Id, parent: null, new Name("Humain"), _world.OwnerId);
    _personality = new(_world.Id, new Name("Courroucé"), _world.OwnerId);
  }

  [Fact(DisplayName = "It should throw ArgumentException when the lineage resides in another world.")]
  public void It_should_throw_ArgumentException_when_the_lineage_resides_in_another_world()
  {
    Lineage lineage = new(WorldId.NewId(), parent: null, new Name("Elfe"), UserId.NewId());

    var exception = Assert.Throws<ArgumentException>(
      () => new Character(_world.Id, new Name("Heracles Aetos"), player: null, lineage, height: 1.84, weight: 84.6, age: 30, _personality, _world.OwnerId)
    );
    Assert.StartsWith("The lineage does not reside in the same world as the character.", exception.Message);
    Assert.Equal("lineage", exception.ParamName);
  }

  [Fact(DisplayName = "It should throw ArgumentException when the personality resides in another world.")]
  public void It_should_throw_ArgumentException_when_the_personality_resides_in_another_world()
  {
    Personality personality = new(WorldId.NewId(), new Name("Courroucé"), UserId.NewId());

    var exception = Assert.Throws<ArgumentException>(
      () => new Character(_world.Id, new Name("Heracles Aetos"), player: null, _lineage, height: 1.84, weight: 84.6, age: 30, personality, _world.OwnerId)
    );
    Assert.StartsWith("The personality does not reside in the same world as the character.", exception.Message);
    Assert.Equal("personality", exception.ParamName);
  }

  [Theory(DisplayName = "It should throw ArgumentOutOfRangeException when the age is not stricly positive.")]
  [InlineData(0)]
  [InlineData(-30)]
  public void It_should_throw_ArgumentOutOfRangeException_when_the_age_is_not_stricly_positive(int age)
  {
    var exception = Assert.Throws<ArgumentOutOfRangeException>(
      () => new Character(_world.Id, new Name("Heracles Aetos"), player: null, _lineage, height: 1.84, weight: 84.6, age, _personality, _world.OwnerId)
    );
    Assert.Equal("age", exception.ParamName);
  }

  [Theory(DisplayName = "It should throw ArgumentOutOfRangeException when the height is not stricly positive.")]
  [InlineData(0)]
  [InlineData(-1.84)]
  public void It_should_throw_ArgumentOutOfRangeException_when_the_height_is_not_stricly_positive(double height)
  {
    var exception = Assert.Throws<ArgumentOutOfRangeException>(
      () => new Character(_world.Id, new Name("Heracles Aetos"), player: null, _lineage, height, weight: 84.6, age: 30, _personality, _world.OwnerId)
    );
    Assert.Equal("height", exception.ParamName);
  }

  [Theory(DisplayName = "It should throw ArgumentOutOfRangeException when the weight is not stricly positive.")]
  [InlineData(0)]
  [InlineData(-84.6)]
  public void It_should_throw_ArgumentOutOfRangeException_when_the_weight_is_not_stricly_positive(double weight)
  {
    var exception = Assert.Throws<ArgumentOutOfRangeException>(
      () => new Character(_world.Id, new Name("Heracles Aetos"), player: null, _lineage, height: 1.84, weight, age: 30, _personality, _world.OwnerId)
    );
    Assert.Equal("weight", exception.ParamName);
  }
}
