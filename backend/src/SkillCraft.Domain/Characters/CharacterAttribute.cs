using SkillCraft.Contracts.Characters;

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
    Modifier = CharacterHelper.CalculateModifier(score);

    TemporaryScore = temporaryScore ?? score;
    TemporaryModifier = CharacterHelper.CalculateModifier(TemporaryScore);
  }
}
