using GraphQL.Types;
using Logitar.Portal.Contracts.Search;

namespace SkillCraft.GraphQL.Languages;

internal class ScriptSearchResultsGraphType : ObjectGraphType<SearchResults<string>>
{
  public ScriptSearchResultsGraphType()
  {
    Name = "ScriptSearchResults";
    Description = "Represents the results of a script search.";

    Field(x => x.Items, type: typeof(NonNullGraphType<ListGraphType<NonNullGraphType<StringGraphType>>>))
      .Description("The list of matching scripts.");
    Field(x => x.Total)
      .Description("The total number of matching scripts.");
  }
}
