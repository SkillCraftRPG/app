namespace SkillCraft.Contracts.Characters;

public record CharacterAttributeModel
{
  public int Score { get; set; }
  public int Modifier { get; set; }

  public int TemporaryScore { get; set; }
  public int TemporaryModifier { get; set; }

  public CharacterAttributeModel()
  {
  }

  public CharacterAttributeModel(int score, int? temporaryScore = null)
  {
    Score = score;
    Modifier = CharacterHelper.CalculateModifier(score);

    TemporaryScore = temporaryScore ?? score;
    TemporaryModifier = CharacterHelper.CalculateModifier(TemporaryScore);
  }
}
