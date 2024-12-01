namespace SkillCraft.Contracts.Characters;

public static class CharacterHelper
{
  public static int CalculateModifier(int score) => (int)(Math.Floor(score / 2.0) - 5.0);
}
