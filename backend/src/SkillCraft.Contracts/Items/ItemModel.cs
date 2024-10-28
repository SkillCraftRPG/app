using Logitar.Portal.Contracts;
using SkillCraft.Contracts.Items.Properties;
using SkillCraft.Contracts.Worlds;

namespace SkillCraft.Contracts.Items;

public class ItemModel : Aggregate
{
  public WorldModel World { get; set; }

  public string Name { get; set; }
  public string? Description { get; set; }

  public double? Value { get; set; }
  public double? Weight { get; set; }

  public ItemCategory Category { get; set; }
  public ConsumablePropertiesModel? Consumable { get; set; }
  public ContainerPropertiesModel? Container { get; set; }
  public DevicePropertiesModel? Device { get; set; }
  public EquipmentPropertiesModel? Equipment { get; set; }
  public MiscellaneousPropertiesModel? Miscellaneous { get; set; }
  public MoneyPropertiesModel? Money { get; set; }
  public WeaponPropertiesModel? Weapon { get; set; }

  public bool IsAttunementRequired { get; set; }

  public ItemModel() : this(new WorldModel(), string.Empty)
  {
  }

  public ItemModel(WorldModel world, string name)
  {
    World = world;

    Name = name;
  }

  public override string ToString() => $"{Name} | {base.ToString()}";
}
