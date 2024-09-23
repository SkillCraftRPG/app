using SkillCraft.Contracts;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Domain.Talents;

[Trait(Traits.Category, Categories.Unit)]
public class TalentTests
{
  private readonly Talent _talent = new(WorldId.NewId(), tier: 1, new Name("Cuirassé"), UserId.NewId());

  [Fact(DisplayName = "ctor: it should create a talent with a skill")]
  public void ctor_it_should_create_a_talent_with_a_skill()
  {
    Talent talent = new(WorldId.NewId(), tier: 0, new Name("Mêlée"), UserId.NewId());
    Assert.Equal(Skill.Melee, talent.Skill);
  }

  [Fact(DisplayName = "ctor: it should create a talent without skill")]
  public void ctor_it_should_create_a_talent_without_skill()
  {
    Assert.Null(_talent.Skill);
  }

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

  [Fact(DisplayName = "It should throw ArgumentOutOfRangeException when the tier is not valid.")]
  public void It_should_throw_ArgumentOutOfRangeException_when_the_tier_is_not_valid()
  {
    var exception = Assert.Throws<ArgumentOutOfRangeException>(() => new Talent(_talent.WorldId, tier: 10, _talent.Name, new UserId(_talent.CreatedBy)));
    Assert.Equal("tier", exception.ParamName);
  }

  [Fact(DisplayName = "Name: it should nullify the skill.")]
  public void Name_it_should_nullify_the_skill()
  {
    Talent talent = new(WorldId.NewId(), tier: 0, new Name("Furtivité"), UserId.NewId());
    Assert.NotNull(talent.Skill);

    talent.Name = new Name("Esquive");
    Assert.Null(talent.Skill);
  }

  [Fact(DisplayName = "Name: it should set the correct skill.")]
  public void Name_it_should_set_the_correct_skill()
  {
    _talent.Name = new Name("Perception");
    Assert.Equal(Skill.Perception, _talent.Skill);
  }
}
