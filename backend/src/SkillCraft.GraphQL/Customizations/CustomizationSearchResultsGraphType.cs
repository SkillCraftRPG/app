using SkillCraft.Contracts.Customizations;
using SkillCraft.GraphQL.Search;

namespace SkillCraft.GraphQL.Customizations;

internal class CustomizationSearchResultsGraphType : SearchResultsGraphType<CustomizationGraphType, CustomizationModel>
{
  public CustomizationSearchResultsGraphType() : base("CustomizationSearchResults", "Represents the results of a customization search.")
  {
  }
}
