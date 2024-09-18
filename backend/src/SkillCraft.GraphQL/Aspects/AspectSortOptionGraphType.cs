using GraphQL.Types;
using SkillCraft.Contracts.Aspects;
using SkillCraft.GraphQL.Search;

namespace SkillCraft.GraphQL.Aspects;

internal class AspectSortOptionGraphType : SortOptionInputGraphType<AspectSortOption>
{
  public AspectSortOptionGraphType() : base()
  {
    Field(x => x.Field, type: typeof(NonNullGraphType<AspectSortGraphType>))
      .Description("The field on which to apply the sort.");
  }
}
