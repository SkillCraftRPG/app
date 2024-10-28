namespace SkillCraft.Contracts.Items.Properties;

public record WeaponDamageModel
{
  public string Roll { get; set; }
  public DamageType Type { get; set; }

  public WeaponDamageModel() : this(string.Empty, default)
  {
  }

  public WeaponDamageModel(string roll, DamageType type)
  {
    Roll = roll;
    Type = type;
  }
}
