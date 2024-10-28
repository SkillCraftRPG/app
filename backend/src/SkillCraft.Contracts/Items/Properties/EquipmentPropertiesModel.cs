namespace SkillCraft.Contracts.Items.Properties;

public record EquipmentPropertiesModel : IEquipmentProperties
{
  public int Defense { get; set; }
  public int? Resistance { get; set; }
  public EquipmentTrait[] Traits { get; set; } = [];
}
