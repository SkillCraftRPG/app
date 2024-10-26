using SkillCraft.Contracts.Items.Properties;

namespace SkillCraft.Contracts.Items;

public record UpdateItemPayload
{
  public string? Name { get; set; }
  public Change<string>? Description { get; set; }

  public Change<double?>? Value { get; set; }
  public Change<double?>? Weight { get; set; }

  public ConsumablePropertiesModel? Consumable { get; set; }
  public ContainerPropertiesModel? Container { get; set; }
  public DevicePropertiesModel? Device { get; set; }
  public EquipmentPropertiesModel? Equipment { get; set; }
  public MiscellaneousPropertiesModel? Miscellaneous { get; set; }
  public MoneyPropertiesModel? Money { get; set; }
  public WeaponPropertiesModel? Weapon { get; set; }
}
