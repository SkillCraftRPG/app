namespace SkillCraft.Domain.Characters;

public record CharacterAttribute
{
  public int Score { get; }
  public int Modifier { get; }

  public int TemporaryScore { get; }
  public int TemporaryModifier { get; }

  public CharacterAttribute(int score, int? temporaryScore = null)
  {
    Score = score;
    Modifier = CalculateModifier(score);

    TemporaryScore = temporaryScore ?? score;
    TemporaryModifier = CalculateModifier(TemporaryScore);
  }

  private static int CalculateModifier(int score) => (int)(Math.Floor(score / 2.0) - 5.0);
}
