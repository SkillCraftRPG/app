using SkillCraft.Contracts;

namespace SkillCraft.Domain.Talents;

[Trait(Traits.Category, Categories.Unit)]
public class TalentHelperTests
{
  [Fact(DisplayName = "It should return null when the skill could not be found.")]
  public void It_should_return_null_when_the_skill_could_not_be_found()
  {
    Assert.Null(TalentHelper.TryGetSkill(new Name("Formation martiale")));
  }

  [Fact(DisplayName = "It should return the English translated skill.")]
  public void It_should_return_the_English_translated_skill()
  {
    Assert.Equal(Skill.Medicine, TalentHelper.TryGetSkill(new Name(Skill.Medicine.ToString())));
  }

  [Fact(DisplayName = "It should return the French translated skill.")]
  public void It_should_return_the_French_translated_skill()
  {
    Assert.Equal(Skill.Linguistics, TalentHelper.TryGetSkill(new Name("Linguistique")));
  }
}
