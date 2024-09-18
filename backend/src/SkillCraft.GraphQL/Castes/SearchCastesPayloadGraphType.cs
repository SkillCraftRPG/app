using GraphQL.Types;
using SkillCraft.Contracts.Castes;
using SkillCraft.GraphQL.Search;

namespace SkillCraft.GraphQL.Castes;

internal class SearchCastesPayloadGraphType : SearchPayloadInputGraphType<SearchCastesPayload>
{
  public SearchCastesPayloadGraphType() : base()
  {
    Field(x => x.Sort, type: typeof(NonNullGraphType<ListGraphType<NonNullGraphType<CasteSortOptionGraphType>>>))
      .DefaultValue([])
      .Description("The sort parameters of the search.");
  }
}
