using GraphQL.Types;
using SkillCraft.Contracts.Aspects;
using SkillCraft.GraphQL.Search;

namespace SkillCraft.GraphQL.Aspects;

internal class SearchAspectsPayloadGraphType : SearchPayloadInputGraphType<SearchAspectsPayload>
{
  public SearchAspectsPayloadGraphType() : base()
  {
    Field(x => x.Attribute, type: typeof(AttributeGraphType))
      .Description("TODO(fpion): document");
    Field(x => x.Skill, type: typeof(SkillGraphType))
      .Description("TODO(fpion): document");

    Field(x => x.Sort, type: typeof(NonNullGraphType<ListGraphType<NonNullGraphType<AspectSortOptionGraphType>>>))
      .DefaultValue([])
      .Description("The sort parameters of the search.");
  }
}
