using SkillCraft.Contracts.Lineages;
using SkillCraft.GraphQL.Search;

namespace SkillCraft.GraphQL.Lineages;

internal class LineageSearchResultsGraphType : SearchResultsGraphType<LineageGraphType, LineageModel>
{
  public LineageSearchResultsGraphType() : base("LineageSearchResults", "Represents the results of a lineage search.")
  {
  }
}
