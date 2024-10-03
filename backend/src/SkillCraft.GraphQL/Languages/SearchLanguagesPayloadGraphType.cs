using GraphQL.Types;
using SkillCraft.Contracts.Languages;
using SkillCraft.GraphQL.Search;

namespace SkillCraft.GraphQL.Languages;

internal class SearchLanguagesPayloadGraphType : SearchPayloadInputGraphType<SearchLanguagesPayload>
{
  public SearchLanguagesPayloadGraphType() : base()
  {
    Field(x => x.Script)
      .Description("When specified, only languages written using this script will match.");

    Field(x => x.Sort, type: typeof(NonNullGraphType<ListGraphType<NonNullGraphType<LanguageSortOptionGraphType>>>))
      .DefaultValue([])
      .Description("The sort parameters of the search.");
  }
}
