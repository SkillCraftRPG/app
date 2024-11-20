using GraphQL.Types;
using SkillCraft.Contracts.Natures;
using SkillCraft.GraphQL.Customizations;
using SkillCraft.GraphQL.Worlds;

namespace SkillCraft.GraphQL.Natures;

internal class NatureGraphType : AggregateGraphType<NatureModel>
{
  public NatureGraphType() : base("Represents a character nature.")
  {
    Field(x => x.Name)
      .Description("The display name of the nature.");
    Field(x => x.Description)
      .Description("The description of the nature.");

    Field(x => x.Attribute, type: typeof(AttributeGraphType))
      .Description("The attribute to which this nature grants a bonus.");
    Field(x => x.Gift, type: typeof(CustomizationGraphType))
      .Description("The gift granted to characters with this nature.");

    Field(x => x.World, type: typeof(NonNullGraphType<WorldGraphType>))
      .Description("The world in which the nature resides.");
  }
}
