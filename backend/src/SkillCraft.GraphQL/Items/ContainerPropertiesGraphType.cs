using GraphQL.Types;
using Logitar;
using SkillCraft.Contracts.Items.Properties;

namespace SkillCraft.GraphQL.Items;

internal class ContainerPropertiesGraphType : ObjectGraphType<ContainerPropertiesModel>
{
  public ContainerPropertiesGraphType()
  {
    Name = nameof(ContainerPropertiesModel).Remove("Model");
    Description = "Represents the properties of container items.";

    Field(x => x.Capacity)
      .Description("The capacity of the container, in kilograms (kg).");
    Field(x => x.Volume)
      .Description("The volume of the container, in liters (L).");
  }
}
