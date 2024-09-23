using GraphQL.Types;
using SkillCraft.Contracts.Parties;
using SkillCraft.GraphQL.Search;

namespace SkillCraft.GraphQL.Parties;

internal class SearchPartiesPayloadGraphType : SearchPayloadInputGraphType<SearchPartiesPayload>
{
  public SearchPartiesPayloadGraphType() : base()
  {
    Field(x => x.Sort, type: typeof(NonNullGraphType<ListGraphType<NonNullGraphType<PartySortOptionGraphType>>>))
      .DefaultValue([])
      .Description("The sort parameters of the search.");
  }
}
