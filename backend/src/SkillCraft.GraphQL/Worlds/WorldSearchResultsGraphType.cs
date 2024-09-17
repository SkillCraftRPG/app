using SkillCraft.Contracts.Worlds;
using SkillCraft.GraphQL.Search;

namespace SkillCraft.GraphQL.Worlds;

internal class WorldSearchResultsGraphType : SearchResultsGraphType<WorldGraphType, WorldModel>
{
  public WorldSearchResultsGraphType() : base("WorldSearchResults", "Represents the results of a world search.")
  {
  }
}
