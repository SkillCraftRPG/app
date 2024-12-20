﻿using SkillCraft.Contracts.Items.Properties;

namespace SkillCraft.Contracts.Items;

public record CreateOrReplaceItemPayload
{
  public string Name { get; set; }
  public string? Description { get; set; }

  public double? Value { get; set; }
  public double? Weight { get; set; }

  public bool IsAttunementRequired { get; set; }

  public ConsumablePropertiesModel? Consumable { get; set; }
  public ContainerPropertiesModel? Container { get; set; }
  public DevicePropertiesModel? Device { get; set; }
  public EquipmentPropertiesModel? Equipment { get; set; }
  public MiscellaneousPropertiesModel? Miscellaneous { get; set; }
  public MoneyPropertiesModel? Money { get; set; }
  public WeaponPropertiesModel? Weapon { get; set; }

  public CreateOrReplaceItemPayload() : this(string.Empty)
  {
  }

  public CreateOrReplaceItemPayload(string name)
  {
    Name = name;
  }
}
