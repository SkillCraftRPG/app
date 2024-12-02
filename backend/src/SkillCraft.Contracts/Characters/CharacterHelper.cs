namespace SkillCraft.Contracts.Characters;

public static class CharacterHelper
{
  public static int CalculateMaximumSkillRank(int tier) => tier switch
  {
    3 => 14,
    2 => 9,
    1 => 5,
    _ => 2,
  };

  public static int CalculateModifier(int score) => (int)(Math.Floor(score / 2.0) - 5.0);
}
