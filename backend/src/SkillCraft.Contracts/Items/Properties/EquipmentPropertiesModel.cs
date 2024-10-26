namespace SkillCraft.Contracts.Items.Properties;

public record EquipmentPropertiesModel : IEquipmentProperties
{
  public int? Defense { get; set; } // TODO(fpion): nullability
  public int? Resistance { get; set; } // TODO(fpion): nullability
  // TODO(fpion): Properties
}
