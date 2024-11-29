namespace SkillCraft.Domain.Characters;

public static class CharacterHelper
{
  public static bool CanLevelUp(int level, int experience) // TODO(fpion): unit tests
  {
    return level < 20 && experience >= ExperienceTable.GetTotalExperience(level + 1);
  }

  public static int GetMaximumRank(int tier) => tier switch // TODO(fpion): unit tests
  {
    3 => 14,
    2 => 9,
    1 => 5,
    _ => 2,
  };
}
