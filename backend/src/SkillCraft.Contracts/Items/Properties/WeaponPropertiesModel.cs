namespace SkillCraft.Contracts.Items.Properties;

public record WeaponPropertiesModel : IWeaponProperties
{
  public int? Attack { get; set; } // TODO(fpion): nullability
  public string? DamageRoll { get; set; } // TODO(fpion): nullability
  // TODO(fpion): DamageType
  public int? Resistance { get; } // TODO(fpion): nullability
  // TODO(fpion): Properties
}
