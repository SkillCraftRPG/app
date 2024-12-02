namespace SkillCraft.Contracts.Characters;

public record CharacterSkillModel
{
  public bool IsTrained { get; set; }
  public int Total { get; set; }

  public CharacterSkillModel()
  {
  }

  public CharacterSkillModel(int total, bool isTrained = false)
  {
    Total = total;
    IsTrained = isTrained;
  }
}
