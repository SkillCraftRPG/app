namespace SkillCraft.Domain.Characters;

internal static class ExperienceTable
{
  private static readonly int[] _requiredExperience =
  [
    100, 300, 700, 1300, 2100, 3100, 4300, 5700, 7300, 9100,
    11100, 13300, 15700, 18300, 21100, 24100, 27300, 30700, 34300, 38100
  ];

  public static int GetTotalExperience(int level)
  {
    if (level < 0 || level > 20)
    {
      throw new ArgumentOutOfRangeException(nameof(level), "The value must be comprised between 0 and 20. The boundaries are inclusive.");
    }
    int total = 0;
    for (int i = 0; i < level; i++)
    {
      total += _requiredExperience[i];
    }
    return total;
  }
}
