using GraphQL.Types;
using SkillCraft.Contracts.Personalities;
using SkillCraft.GraphQL.Search;

namespace SkillCraft.GraphQL.Personalities;

internal class SearchPersonalitiesPayloadGraphType : SearchPayloadInputGraphType<SearchPersonalitiesPayload>
{
  public SearchPersonalitiesPayloadGraphType() : base()
  {
    Field(x => x.Sort, type: typeof(NonNullGraphType<ListGraphType<NonNullGraphType<PersonalitySortOptionGraphType>>>))
      .DefaultValue([])
      .Description("The sort parameters of the search.");
  }
}
