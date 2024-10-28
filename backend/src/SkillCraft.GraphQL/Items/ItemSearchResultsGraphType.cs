using SkillCraft.Contracts.Items;
using SkillCraft.GraphQL.Search;

namespace SkillCraft.GraphQL.Items;

internal class ItemSearchResultsGraphType : SearchResultsGraphType<ItemGraphType, ItemModel>
{
  public ItemSearchResultsGraphType() : base("ItemSearchResults", "Represents the results of an item search.")
  {
  }
}
