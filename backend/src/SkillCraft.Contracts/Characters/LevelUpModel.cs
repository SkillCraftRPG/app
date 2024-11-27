namespace SkillCraft.Contracts.Characters;

public record LevelUpModel : ILevelUp
{
  public Attribute Attribute { get; set; }

  public int Constitution { get; set; }
  public double Initiative { get; set; }
  public int Learning { get; set; }
  public double Power { get; set; }
  public double Precision { get; set; }
  public double Reputation { get; set; }
  public double Strength { get; set; }
}
