using GraphQL.Types;
using Logitar;
using SkillCraft.Contracts.Items.Properties;

namespace SkillCraft.GraphQL.Items;

internal class MoneyPropertiesGraphType : ObjectGraphType<MoneyPropertiesModel>
{
  public MoneyPropertiesGraphType()
  {
    Name = nameof(MoneyPropertiesModel).Remove("Model");
    Description = "Represents the properties of money items.";
  }
}
