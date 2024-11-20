using SkillCraft.Contracts.Natures;
using SkillCraft.GraphQL.Search;

namespace SkillCraft.GraphQL.Natures;

internal class NatureSearchResultsGraphType : SearchResultsGraphType<NatureGraphType, NatureModel>
{
  public NatureSearchResultsGraphType() : base("NatureSearchResults", "Represents the results of a nature search.")
  {
  }
}
