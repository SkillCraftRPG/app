using GraphQL.Types;
using SkillCraft.Contracts.Items.Properties;

namespace SkillCraft.GraphQL.Items;

internal class EquipmentTraitGraphType : EnumerationGraphType<EquipmentTrait>
{
  public EquipmentTraitGraphType()
  {
    Name = nameof(EquipmentTrait);
    Description = "Represents the available equipment traits.";

    AddValue(EquipmentTrait.Bulwark, string.Empty);
    AddValue(EquipmentTrait.Comfort, string.Empty);
    AddValue(EquipmentTrait.Hybrid, string.Empty);
    AddValue(EquipmentTrait.Noisy, string.Empty);
    AddValue(EquipmentTrait.Quilted, string.Empty);
    AddValue(EquipmentTrait.Sturdy, string.Empty);
  }
  private void AddValue(EquipmentTrait value, string description)
  {
    Add(value.ToString(), value, description);
  }
}
