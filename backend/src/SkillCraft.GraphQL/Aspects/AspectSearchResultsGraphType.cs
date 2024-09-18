using SkillCraft.Contracts.Aspects;
using SkillCraft.GraphQL.Search;

namespace SkillCraft.GraphQL.Aspects;

internal class AspectSearchResultsGraphType : SearchResultsGraphType<AspectGraphType, AspectModel>
{
  public AspectSearchResultsGraphType() : base("AspectSearchResults", "Represents the results of an aspect search.")
  {
  }
}
