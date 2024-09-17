using GraphQL.Types;
using SkillCraft.Contracts.Worlds;
using SkillCraft.GraphQL.Search;

namespace SkillCraft.GraphQL.Worlds;

internal class SearchWorldsPayloadGraphType : SearchPayloadInputGraphType<SearchWorldsPayload>
{
  public SearchWorldsPayloadGraphType() : base()
  {
    Field(x => x.Sort, type: typeof(NonNullGraphType<ListGraphType<NonNullGraphType<WorldSortOptionGraphType>>>))
      .DefaultValue([])
      .Description("The sort parameters of the search.");
  }
}
