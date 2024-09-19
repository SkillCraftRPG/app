using GraphQL.Types;
using SkillCraft.Contracts.Lineages;
using SkillCraft.GraphQL.Search;

namespace SkillCraft.GraphQL.Lineages;

internal class SearchLineagesPayloadGraphType : SearchPayloadInputGraphType<SearchLineagesPayload>
{
  public SearchLineagesPayloadGraphType() : base()
  {
    Field(x => x.ParentId)
      .Description("The unique identifier of the parent lineage to match.");

    Field(x => x.Sort, type: typeof(NonNullGraphType<ListGraphType<NonNullGraphType<LineageSortOptionGraphType>>>))
      .DefaultValue([])
      .Description("The sort parameters of the search.");
  }
}
