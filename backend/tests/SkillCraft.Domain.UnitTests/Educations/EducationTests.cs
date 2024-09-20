using SkillCraft.Contracts;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Domain.Educations;

[Trait(Traits.Category, Categories.Unit)]
public class EducationTests
{
  private readonly Education _education = new(WorldId.NewId(), new Name("Judicieux"), UserId.NewId());

  [Fact(DisplayName = "It should throw ArgumentOutOfRangeException when the skill is not defined.")]
  public void It_should_throw_ArgumentOutOfRangeException_when_the_skill_is_not_defined()
  {
    var exception = Assert.Throws<ArgumentOutOfRangeException>(() => _education.Skill = (Skill)(-1));
    Assert.Equal("Skill", exception.ParamName);
  }

  [Fact(DisplayName = "It should throw ArgumentOutOfRangeException when the wealth multiplier is negative.")]
  public void It_should_throw_ArgumentOutOfRangeException_when_the_wealth_multiplier_is_negative()
  {
    var exception = Assert.Throws<ArgumentOutOfRangeException>(() => _education.WealthMultiplier = -10.0);
    Assert.Equal("WealthMultiplier", exception.ParamName);
  }

  [Fact(DisplayName = "It should throw ArgumentOutOfRangeException when the wealth multiplier is zero.")]
  public void It_should_throw_ArgumentOutOfRangeException_when_the_wealth_multiplier_is_zero()
  {
    var exception = Assert.Throws<ArgumentOutOfRangeException>(() => _education.WealthMultiplier = 0.0);
    Assert.Equal("WealthMultiplier", exception.ParamName);
  }
}
