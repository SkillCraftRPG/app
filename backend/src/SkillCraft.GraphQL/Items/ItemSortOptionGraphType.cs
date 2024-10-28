using GraphQL.Types;
using SkillCraft.Contracts.Items;
using SkillCraft.GraphQL.Search;

namespace SkillCraft.GraphQL.Items;

internal class ItemSortOptionGraphType : SortOptionInputGraphType<ItemSortOption>
{
  public ItemSortOptionGraphType() : base()
  {
    Field(x => x.Field, type: typeof(NonNullGraphType<ItemSortGraphType>))
      .Description("The field on which to apply the sort.");
  }
}
