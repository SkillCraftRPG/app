using GraphQL.Types;
using Logitar;
using SkillCraft.Contracts.Items.Properties;

namespace SkillCraft.GraphQL.Items;

internal class MiscellaneousPropertiesGraphType : ObjectGraphType<MiscellaneousPropertiesModel>
{
  public MiscellaneousPropertiesGraphType()
  {
    Name = nameof(MiscellaneousPropertiesModel).Remove("Model");
    Description = "Represents the properties of miscellaneous items.";
  }
}
