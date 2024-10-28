using GraphQL.Types;
using Logitar;
using SkillCraft.Contracts.Items.Properties;

namespace SkillCraft.GraphQL.Items;

internal class ConsumablePropertiesGraphType : ObjectGraphType<ConsumablePropertiesModel>
{
  public ConsumablePropertiesGraphType()
  {
    Name = nameof(ConsumablePropertiesModel).Remove("Model");
    Description = "Represents the properties of consumable items.";
  }
}
