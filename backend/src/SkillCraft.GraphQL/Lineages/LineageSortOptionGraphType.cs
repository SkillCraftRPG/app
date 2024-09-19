using GraphQL.Types;
using SkillCraft.Contracts.Lineages;
using SkillCraft.GraphQL.Search;

namespace SkillCraft.GraphQL.Lineages;

internal class LineageSortOptionGraphType : SortOptionInputGraphType<LineageSortOption>
{
  public LineageSortOptionGraphType() : base()
  {
    Field(x => x.Field, type: typeof(NonNullGraphType<LineageSortGraphType>))
      .Description("The field on which to apply the sort.");
  }
}
