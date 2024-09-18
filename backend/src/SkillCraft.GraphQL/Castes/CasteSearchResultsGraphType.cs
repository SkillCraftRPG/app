using SkillCraft.Contracts.Castes;
using SkillCraft.GraphQL.Search;

namespace SkillCraft.GraphQL.Castes;

internal class CasteSearchResultsGraphType : SearchResultsGraphType<CasteGraphType, CasteModel>
{
  public CasteSearchResultsGraphType() : base("CasteSearchResults", "Represents the results of a caste search.")
  {
  }
}
