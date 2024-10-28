using GraphQL.Types;
using Logitar;
using SkillCraft.Contracts.Items.Properties;

namespace SkillCraft.GraphQL.Items;

internal class EquipmentPropertiesGraphType : ObjectGraphType<EquipmentPropertiesModel>
{
  public EquipmentPropertiesGraphType()
  {
    Name = nameof(EquipmentPropertiesModel).Remove("Model");
    Description = "Represents the properties of equipment items.";
  }
}
