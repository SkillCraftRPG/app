using GraphQL.Types;
using SkillCraft.Contracts.Items;

namespace SkillCraft.GraphQL.Items;

internal class ItemSortGraphType : EnumerationGraphType<ItemSort>
{
  public ItemSortGraphType()
  {
    Name = nameof(ItemSort);
    Description = "Represents the available item fields for sorting.";

    AddValue(ItemSort.CreatedOn, "The items will be sorted by their creation date and time.");
    AddValue(ItemSort.Name, "The items will be sorted by their display name.");
    AddValue(ItemSort.UpdatedOn, "The items will be sorted by their latest update date and time.");
    AddValue(ItemSort.Value, "The items will be sorted by their monetary value.");
    AddValue(ItemSort.Weight, "The items will be sorted by their weight, in kilograms (kg).");
  }
  private void AddValue(ItemSort value, string description)
  {
    Add(value.ToString(), value, description);
  }
}
