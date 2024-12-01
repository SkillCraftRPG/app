namespace SkillCraft.Contracts.Characters;

public record TalentPointsModel
{
  public int Available { get; set; }
  public int Spent { get; set; }
  public int Remaining { get; set; }

  public TalentPointsModel()
  {
  }

  public TalentPointsModel(CharacterModel character)
  {
    Available = 8 + (character.Level * 4);
    Spent = character.Talents.Sum(talent => talent.Cost);
    Remaining = Available - Spent;
  }
}
