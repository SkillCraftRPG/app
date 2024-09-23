using SkillCraft.Contracts;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Domain.Talents;

[Trait(Traits.Category, Categories.Unit)]
public class TalentTests
{
  private readonly Talent _talent = new(WorldId.NewId(), tier: 1, new Name("Cuirassé"), UserId.NewId());

  [Fact(DisplayName = "It should throw ArgumentException when requiring a talent from another world.")]
  public void It_should_throw_ArgumentException_when_setting_a_gift_from_another_world()
  {
    Talent talent = new(WorldId.NewId(), tier: 0, new Name("Formation martiale"), UserId.NewId());
    var exception = Assert.Throws<ArgumentException>(() => _talent.SetRequiredTalent(talent));
    Assert.StartsWith("The required talent does not reside in the same world as the requiring talent.", exception.Message);
    Assert.Equal("requiredTalent", exception.ParamName);
  }

  [Fact(DisplayName = "It should throw ArgumentException when requiring a talent of a higher tier.")]
  public void It_should_throw_ArgumentException_when_setting_a_customization_that_is_not_a_gift()
  {
    Talent talent = new(_talent.WorldId, tier: 2, new Name("Protection"), new UserId(_talent.CreatedBy));
    var exception = Assert.Throws<ArgumentException>(() => _talent.SetRequiredTalent(talent));
    Assert.StartsWith("The required talent tier must be inferior or equal to the requiring talent tier.", exception.Message);
    Assert.Equal("requiredTalent", exception.ParamName);
  }

  [Fact(DisplayName = "It should throw ArgumentOutOfRangeException when the skill is not valid.")]
  public void It_should_throw_ArgumentOutOfRangeException_when_the_skill_is_not_valid()
  {
    var exception = Assert.Throws<ArgumentOutOfRangeException>(() => _talent.Skill = (Skill)(-1));
    Assert.Equal("Skill", exception.ParamName);
  }

  [Fact(DisplayName = "It should throw ArgumentOutOfRangeException when the tier is not valid.")]
  public void It_should_throw_ArgumentOutOfRangeException_when_the_tier_is_not_valid()
  {
    var exception = Assert.Throws<ArgumentOutOfRangeException>(() => new Talent(_talent.WorldId, tier: 10, _talent.Name, new UserId(_talent.CreatedBy)));
    Assert.Equal("tier", exception.ParamName);
  }
}
