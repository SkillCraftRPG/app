using SkillCraft.Contracts.Languages;
using SkillCraft.GraphQL.Search;

namespace SkillCraft.GraphQL.Languages;

internal class LanguageSearchResultsGraphType : SearchResultsGraphType<LanguageGraphType, LanguageModel>
{
  public LanguageSearchResultsGraphType() : base("LanguageSearchResults", "Represents the results of a language search.")
  {
  }
}
