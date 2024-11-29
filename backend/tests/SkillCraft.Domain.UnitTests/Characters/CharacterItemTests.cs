using SkillCraft.Contracts;
using SkillCraft.Domain.Items;
using SkillCraft.Domain.Items.Properties;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Domain.Characters;

[Trait(Traits.Category, Categories.Unit)]
public class CharacterItemTests
{
  private readonly WorldMock _world = new();

  private readonly Item _commonClothes;
  private readonly Item _crystalOfDoom;
  private readonly Item _denier;
  private readonly Item _otherWorldItem;
  private readonly Item _pouch;

  private readonly Character _character;

  public CharacterItemTests()
  {
    _commonClothes = new(_world.Id, new Name("Vêtements communs"), new EquipmentProperties(defense: 0, resistance: null, traits: []), _world.OwnerId);
    _crystalOfDoom = new(_world.Id, new Name("Cristal de Damnation"), new MiscellaneousProperties(), _world.OwnerId);
    _denier = new(_world.Id, new Name("Denier"), new MoneyProperties(), _world.OwnerId)
    {
      Value = 1.0,
      Weight = 0.005
    };
    _denier.Update(_world.OwnerId);
    _otherWorldItem = new(WorldId.NewId(), new Name("Denier"), new MoneyProperties(), UserId.NewId());
    _pouch = new(_world.Id, new Name("Bourse"), new ContainerProperties(capacity: 2.5, volume: null), _world.OwnerId);

    _character = new CharacterBuilder(_world).Build();
  }

  [Fact(DisplayName = "AddItem: it should add a new item with options.")]
  public void AddItem_it_should_add_a_new_item_with_options()
  {
    SetItemOptions options = new()
    {
      Quantity = 2,
      IsIdentified = false,
      NameOverride = new Change<Name>(new Name("Cristal sombre")),
      DescriptionOverride = new Change<Description>(new Description("Un cristal opaque d’un noir profond."))
    };
    _character.AddItem(_crystalOfDoom, options, _world.OwnerId);

    CharacterItem item = Assert.Single(_character.Inventory.Values);
    Assert.Equal(_crystalOfDoom.Id, item.ItemId);
    Assert.Null(item.ContainingItemId);
    Assert.Equal(options.Quantity, item.Quantity);
    Assert.Null(item.IsAttuned);
    Assert.False(item.IsEquipped);
    Assert.Equal(options.IsIdentified, item.IsIdentified);
    Assert.Null(item.IsProficient);
    Assert.Null(item.Skill);
    Assert.Null(item.RemainingCharges);
    Assert.Null(item.RemainingResistance);
    Assert.Equal(options.NameOverride.Value, item.NameOverride);
    Assert.Equal(options.DescriptionOverride.Value, item.DescriptionOverride);
    Assert.Null(item.ValueOverride);
  }

  [Fact(DisplayName = "AddItem: it should add a new item without options.")]
  public void AddItem_it_should_add_a_new_item_without_options()
  {
    _character.AddItem(_commonClothes, _world.OwnerId);

    CharacterItem item = Assert.Single(_character.Inventory.Values);
    Assert.Equal(_commonClothes.Id, item.ItemId);
    Assert.Null(item.ContainingItemId);
    Assert.Equal(1, item.Quantity);
    Assert.Null(item.IsAttuned);
    Assert.False(item.IsEquipped);
    Assert.True(item.IsIdentified);
    Assert.Null(item.IsProficient);
    Assert.Null(item.Skill);
    Assert.Null(item.RemainingCharges);
    Assert.Null(item.RemainingResistance);
    Assert.Null(item.NameOverride);
    Assert.Null(item.DescriptionOverride);
    Assert.Null(item.ValueOverride);
  }

  [Fact(DisplayName = "AddItem: it should throw ArgumentException when the item resides in another world.")]
  public void AddItem_it_should_throw_ArgumentException_when_the_item_resides_in_another_world()
  {
    var exception = Assert.Throws<ArgumentException>(() => _character.AddItem(_otherWorldItem, _world.OwnerId));
    Assert.StartsWith("The item does not reside in the same world as the character.", exception.Message);
    Assert.Equal("item", exception.ParamName);
  }

  [Fact(DisplayName = "SetItem: it should add a new item.")]
  public void SetItem_it_should_add_a_new_item()
  {
    Guid id = Guid.NewGuid();
    _character.SetItem(id, _pouch, _world.OwnerId);

    Assert.Equal(id, Assert.Single(_character.Inventory.Keys));

    CharacterItem item = Assert.Single(_character.Inventory.Values);
    Assert.Equal(_pouch.Id, item.ItemId);
    Assert.Null(item.ContainingItemId);
    Assert.Equal(1, item.Quantity);
    Assert.Null(item.IsAttuned);
    Assert.False(item.IsEquipped);
    Assert.True(item.IsIdentified);
    Assert.Null(item.IsProficient);
    Assert.Null(item.Skill);
    Assert.Null(item.RemainingCharges);
    Assert.Null(item.RemainingResistance);
    Assert.Null(item.NameOverride);
    Assert.Null(item.DescriptionOverride);
    Assert.Null(item.ValueOverride);
  }

  [Fact(DisplayName = "SetItem: it should not do anything when the item did not change.")]
  public void SetItem_it_should_not_do_anything_when_the_item_did_not_change()
  {
    Guid id = Guid.NewGuid();
    _character.SetItem(id, _pouch, _world.OwnerId);
    _character.ClearChanges();

    _character.SetItem(id, _pouch, _world.OwnerId);
    Assert.Empty(_character.Changes);
    Assert.False(_character.HasChanges);
  }

  [Fact(DisplayName = "SetItem: it should throw ArgumentException when the item resides in another world.")]
  public void SetItem_it_should_throw_ArgumentException_when_the_item_resides_in_another_world()
  {
    var exception = Assert.Throws<ArgumentException>(() => _character.SetItem(Guid.NewGuid(), _otherWorldItem, _world.OwnerId));
    Assert.StartsWith("The item does not reside in the same world as the character.", exception.Message);
    Assert.Equal("item", exception.ParamName);
  }

  [Fact(DisplayName = "SetItem: it should update an existing item.")]
  public void SetItem_it_should_update_an_existing_item()
  {
    _character.AddItem(_pouch, _world.OwnerId);
    Guid pouchId = Assert.Single(_character.Inventory.Keys);

    Guid denierId = Guid.NewGuid();
    _character.SetItem(denierId, _denier, _world.OwnerId);

    SetItemOptions options = new()
    {
      ContainingItemId = new Change<Guid?>(pouchId),
      Quantity = 100
    };
    _character.SetItem(denierId, _denier, options, _world.OwnerId);

    Assert.Equal(2, _character.Inventory.Count);

    Assert.Contains(_character.Inventory, i => i.Key == pouchId && i.Value.ItemId == _pouch.Id);

    Assert.Contains(denierId, _character.Inventory.Keys);
    CharacterItem item = _character.Inventory[denierId];
    Assert.Equal(_denier.Id, item.ItemId);
    Assert.Equal(options.ContainingItemId.Value, item.ContainingItemId);
    Assert.Equal(options.Quantity, item.Quantity);
    Assert.Null(item.IsAttuned);
    Assert.False(item.IsEquipped);
    Assert.True(item.IsIdentified);
    Assert.Null(item.IsProficient);
    Assert.Null(item.Skill);
    Assert.Null(item.RemainingCharges);
    Assert.Null(item.RemainingResistance);
    Assert.Null(item.NameOverride);
    Assert.Null(item.DescriptionOverride);
    Assert.Null(item.ValueOverride);
  }
}
