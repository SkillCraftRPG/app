namespace SkillCraft.Contracts.Items.Properties;

public record EquipmentPropertiesModel : IEquipmentProperties
{
  public int Defense { get; set; }
  public int? Resistance { get; set; }
  public List<EquipmentTrait> Traits { get; set; }

  public EquipmentPropertiesModel()
  {
    Traits = [];
  }
}
