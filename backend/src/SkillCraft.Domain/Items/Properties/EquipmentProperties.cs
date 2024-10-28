using FluentValidation;
using SkillCraft.Contracts.Items;
using SkillCraft.Contracts.Items.Properties;

namespace SkillCraft.Domain.Items.Properties;

public record EquipmentProperties : PropertiesBase, IEquipmentProperties
{
  public override ItemCategory Category { get; } = ItemCategory.Equipment;

  public int Defense { get; }
  public int? Resistance { get; }
  private readonly HashSet<EquipmentTrait> _traits;
  public EquipmentTrait[] Traits => [.. _traits];

  public EquipmentProperties(IEquipmentProperties equipment) : this(equipment.Defense, equipment.Resistance, equipment.Traits)
  {
  }

  public EquipmentProperties(int defense, int? resistance, EquipmentTrait[] traits)
  {
    Defense = defense;
    Resistance = resistance;
    _traits = [.. traits];
    new EquipmentPropertiesValidator().ValidateAndThrow(this);
  }

  public bool HasTrait(EquipmentTrait trait) => _traits.Contains(trait);
}
