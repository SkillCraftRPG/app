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

    Field(x => x.Defense)
      .Description("The defense points granted by the equipment.");
    Field(x => x.Resistance)
      .Description("The resistance points of the equipment.");
    Field(x => x.Traits, type: typeof(NonNullGraphType<ListGraphType<NonNullGraphType<EquipmentTraitGraphType>>>))
      .Description("The traits of the equipment.");
  }
}
