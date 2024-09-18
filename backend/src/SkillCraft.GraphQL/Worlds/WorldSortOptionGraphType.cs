using GraphQL.Types;
using SkillCraft.Contracts.Worlds;
using SkillCraft.GraphQL.Search;

namespace SkillCraft.GraphQL.Worlds;

internal class WorldSortOptionGraphType : SortOptionInputGraphType<WorldSortOption>
{
  public WorldSortOptionGraphType() : base()
  {
    Field(x => x.Field, type: typeof(NonNullGraphType<WorldSortGraphType>))
      .Description("The field on which to apply the sort.");
  }
}
