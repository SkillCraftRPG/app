using GraphQL.Types;
using SkillCraft.Contracts.Aspects;
using SkillCraft.GraphQL.Worlds;

namespace SkillCraft.GraphQL.Aspects;

internal class AspectGraphType : AggregateGraphType<AspectModel>
{
  public AspectGraphType() : base("Represents a character aspect.")
  {
    Field(x => x.Name)
      .Description("The display name of the aspect.");
    Field(x => x.Description)
      .Description("The description of the aspect.");

    Field(x => x.Attributes, type: typeof(NonNullGraphType<AttributesGraphType>))
      .Description("The attribute selection of the aspect.");
    Field(x => x.Skills, type: typeof(NonNullGraphType<SkillsGraphType>))
      .Description("The skill selection of the aspect.");

    Field(x => x.World, type: typeof(NonNullGraphType<WorldGraphType>))
      .Description("The world in which the aspect resides.");
  }
}
