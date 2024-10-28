using GraphQL.Types;
using SkillCraft.Contracts.Items;
using SkillCraft.GraphQL.Worlds;

namespace SkillCraft.GraphQL.Items;

internal class ItemGraphType : AggregateGraphType<ItemModel>
{
  public ItemGraphType() : base("Represents a character item.")
  {
    Field(x => x.Name)
      .Description("The display name of the item.");
    Field(x => x.Description)
      .Description("The description of the item.");

    Field(x => x.Value)
      .Description("The value of the item, in the basic currency.");
    Field(x => x.Weight)
      .Description("The weight of the item, in kilograms (kg).");

    Field(x => x.IsAttunementRequired)
      .Description("A value indicating whether or not this item is a magic item requiring attunement.");

    Field(x => x.Category, type: typeof(NonNullGraphType<ItemCategoryGraphType>))
      .Description("The category of the item.");
    Field(x => x.Consumable, type: typeof(ConsumablePropertiesGraphType))
      .Description("The properties of a consumable item.");
    Field(x => x.Container, type: typeof(ContainerPropertiesGraphType))
      .Description("The properties of a container item.");
    Field(x => x.Device, type: typeof(DevicePropertiesGraphType))
      .Description("The properties of a device item.");
    Field(x => x.Equipment, type: typeof(EquipmentPropertiesGraphType))
      .Description("The properties of an equipment item.");
    Field(x => x.Miscellaneous, type: typeof(MiscellaneousPropertiesGraphType))
      .Description("The properties of a miscellaneous item.");
    Field(x => x.Money, type: typeof(MoneyPropertiesGraphType))
      .Description("The properties of a money item.");
    Field(x => x.Weapon, type: typeof(WeaponPropertiesGraphType))
      .Description("The properties of a weapon item.");

    Field(x => x.World, type: typeof(NonNullGraphType<WorldGraphType>))
      .Description("The world in which the item resides.");
  }
}
