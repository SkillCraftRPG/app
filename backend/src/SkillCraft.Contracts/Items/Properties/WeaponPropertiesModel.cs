namespace SkillCraft.Contracts.Items.Properties;

public record WeaponPropertiesModel : IWeaponProperties
{
  public int Attack { get; set; }
  public int? Resistance { get; set; }
  public List<WeaponTrait> Traits { get; set; }

  public List<WeaponDamageModel> Damages { get; set; }
  public List<WeaponDamageModel> VersatileDamages { get; set; }

  public WeaponRangeModel? AmmunitionRange { get; set; }
  public WeaponRangeModel? ThrownRange { get; set; }
  public int? ReloadCount { get; set; }

  public WeaponPropertiesModel()
  {
    Traits = [];

    Damages = [];
    VersatileDamages = [];
  }
}
