using FluentValidation;
using SkillCraft.Contracts.Items;
using SkillCraft.Contracts.Items.Properties;

namespace SkillCraft.Domain.Items.Properties;

public record EquipmentProperties : PropertiesBase, IEquipmentProperties
{
  public override ItemCategory Category { get; } = ItemCategory.Equipment;

  public EquipmentProperties(IEquipmentProperties equipment) : this()
  {
  }

  public EquipmentProperties()
  {
    new EquipmentPropertiesValidator().ValidateAndThrow(this);
  }
}
