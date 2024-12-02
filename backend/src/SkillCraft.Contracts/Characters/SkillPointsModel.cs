namespace SkillCraft.Contracts.Characters;

public record SkillPointsModel
{
  public int Available { get; set; }
  public int Spent { get; set; }
  public int Remaining { get; set; }

  public SkillPointsModel()
  {
  }

  public SkillPointsModel(CharacterModel character)
  {
    Available = character.Statistics.Learning.Value;
    Spent = character.SkillRanks.Sum(skill => skill.Rank);
    Remaining = Available - Spent;
  }
}
