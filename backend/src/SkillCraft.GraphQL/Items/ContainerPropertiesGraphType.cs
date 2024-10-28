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
  }
}
