using GraphQL.Types;
using SkillCraft.Contracts.Items;
using SkillCraft.GraphQL.Search;

namespace SkillCraft.GraphQL.Items;

internal class SearchItemsPayloadGraphType : SearchPayloadInputGraphType<SearchItemsPayload>
{
  public SearchItemsPayloadGraphType() : base()
  {
    Field(x => x.Category, type: typeof(ItemCategoryGraphType))
      .Description("When specified, only items belonging to this category will match.");
    Field(x => x.Value, type: typeof(DoubleFilterGraphType))
      .Description("When specified, only items matching the value filter will match.");
    Field(x => x.Weight, type: typeof(DoubleFilterGraphType))
      .Description("When specified, only items matching the weight filter will match.");

    Field(x => x.Sort, type: typeof(NonNullGraphType<ListGraphType<NonNullGraphType<ItemSortOptionGraphType>>>))
      .DefaultValue([])
      .Description("The sort parameters of the search.");
  }
}
