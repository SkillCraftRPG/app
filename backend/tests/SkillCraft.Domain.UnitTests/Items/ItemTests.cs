using SkillCraft.Contracts;
using SkillCraft.Contracts.Items;
using SkillCraft.Contracts.Items.Properties;
using SkillCraft.Domain.Items.Properties;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Domain.Items;

[Trait(Traits.Category, Categories.Unit)]
public class ItemTests
{
  private readonly UserId _userId = UserId.NewId();
  private readonly WorldId _worldId = WorldId.NewId();
  private readonly Item _item;

  public ItemTests()
  {
    _item = new(_worldId, new Name("Denier"), new MoneyProperties(), _userId);
  }

  [Fact(DisplayName = "It should throw ArgumentException when the category is not defined.")]
  public void It_should_throw_ArgumentException_when_the_category_is_not_defined()
  {
    var exception = Assert.Throws<ArgumentException>(() => new Item(_item.WorldId, _item.Name, new UndefinedProperties(), UserId.NewId()));
    Assert.StartsWith("The category '-1' is not defined.", exception.Message);
    Assert.Equal("properties", exception.ParamName);
  }

  [Fact(DisplayName = "It should throw ArgumentOutOfRangeException when setting a negative value.")]
  public void It_should_throw_ArgumentOutOfRangeException_when_setting_a_negative_value()
  {
    var exception = Assert.Throws<ArgumentOutOfRangeException>(() => _item.Value = -10);
    Assert.Equal("Value", exception.ParamName);
  }

  [Fact(DisplayName = "It should throw ArgumentOutOfRangeException when setting a negative weight.")]
  public void It_should_throw_ArgumentOutOfRangeException_when_setting_a_negative_weight()
  {
    var exception = Assert.Throws<ArgumentOutOfRangeException>(() => _item.Weight = -0.01);
    Assert.Equal("Weight", exception.ParamName);
  }

  [Fact(DisplayName = "It should throw ItemCategoryMismatchException when the category does not match the item.")]
  public void It_should_throw_ItemCategoryMismatchException_when_the_category_does_not_match_the_item()
  {
    EquipmentProperties equipmentProperties = new(defense: 1, resistance: 1, traits: [EquipmentTrait.Comfort]);
    Item equipment = new(_item.WorldId, new Name("Brigandine"), equipmentProperties, _userId);

    Dictionary<ItemCategory, PropertiesBase> propertiesByCategory = new()
    {
      [ItemCategory.Consumable] = new ConsumableProperties(charges: 1, removeWhenEmpty: false, replaceWithItemWhenEmptyId: new ItemId(_item.WorldId, Guid.NewGuid())),
      [ItemCategory.Container] = new ContainerProperties(capacity: 150, volume: 300),
      [ItemCategory.Device] = new DeviceProperties(),
      [ItemCategory.Equipment] = equipmentProperties,
      [ItemCategory.Miscellaneous] = new MiscellaneousProperties(),
      [ItemCategory.Money] = new MoneyProperties(),
      [ItemCategory.Weapon] = new WeaponProperties(
        attack: 5,
        resistance: 24,
        traits: [WeaponTrait.Ammunition, WeaponTrait.Loading, WeaponTrait.Range, WeaponTrait.Reload],
        damages: [new WeaponDamage(new Roll("1d10"), DamageType.Piercing)],
        versatileDamages: [],
        new WeaponRange(8, 24),
        reloadCount: 6)
    };

    foreach (ItemCategory category in Enum.GetValues<ItemCategory>())
    {
      PropertiesBase properties = propertiesByCategory[category];
      Item item = category == ItemCategory.Money ? equipment : _item;

      MethodInfo? setProperties = item.GetType().GetMethod("SetProperties", BindingFlags.Instance | BindingFlags.Public, [properties.GetType(), typeof(UserId)]);
      Assert.NotNull(setProperties);

      var exception = Assert.Throws<TargetInvocationException>(() => setProperties.Invoke(item, [properties, _userId]));
      ItemCategoryMismatchException? itemCategoryMismatch = exception.InnerException as ItemCategoryMismatchException;
      Assert.NotNull(itemCategoryMismatch);
      Assert.Equal(item.WorldId.ToGuid(), itemCategoryMismatch.WorldId);
      Assert.Equal(item.EntityId, itemCategoryMismatch.ItemId);
      Assert.Equal(item.Category, itemCategoryMismatch.ExpectedCategory);
      Assert.Equal(category, itemCategoryMismatch.ActualCategory);
    }
  }
}
