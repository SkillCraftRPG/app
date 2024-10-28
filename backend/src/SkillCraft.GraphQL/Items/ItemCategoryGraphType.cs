using GraphQL.Types;
using SkillCraft.Contracts.Items;

namespace SkillCraft.GraphQL.Items;

internal class ItemCategoryGraphType : EnumerationGraphType<ItemCategory>
{
  public ItemCategoryGraphType()
  {
    Name = nameof(ItemCategory);
    Description = "Represents the available item categories.";

    AddValue(ItemCategory.Consumable, "The category for consumable items, such as torches and potions.");
    AddValue(ItemCategory.Container, "The category for container items, such as vials and sacks.");
    AddValue(ItemCategory.Device, "The category for tools, game sets, musical instruments and similar items.");
    AddValue(ItemCategory.Equipment, "The category for wearable equipment, such as armor, shields and clothes.");
    AddValue(ItemCategory.Miscellaneous, "The category for items not matching any other category.");
    AddValue(ItemCategory.Money, "The category for coins, bank notes and items representing currencies.");
    AddValue(ItemCategory.Weapon, "The category for weapon equipment, such as swords, bows and guns.");
  }
  private void AddValue(ItemCategory value, string description)
  {
    Add(value.ToString(), value, description);
  }
}
