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

    Field(x => x.Charges)
      .Description("The number of charges that the item contains.");
    Field(x => x.RemoveWhenEmpty)
      .Description("A value indicating whether or not the item will be removed when all charges are used.");
    Field(x => x.ReplaceWithItemWhenEmptyId)
      .Description("The identifier of the item that this item will be replaced by when all charges are used.");
  }
}
