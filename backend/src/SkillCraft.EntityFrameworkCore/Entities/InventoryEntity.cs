using SkillCraft.Contracts;
using SkillCraft.Domain.Characters;

namespace SkillCraft.EntityFrameworkCore.Entities;

internal class InventoryEntity
{
  public int InventoryId { get; private set; }
  public Guid Id { get; private set; }

  public CharacterEntity? Character { get; private set; }
  public int CharacterId { get; private set; }
  public ItemEntity? Item { get; private set; }
  public int ItemId { get; private set; }

  public Guid? ContainingItemId { get; private set; }
  public int Quantity { get; private set; }
  public bool? IsAttuned { get; private set; }
  public bool IsEquipped { get; private set; }
  public bool IsIdentified { get; private set; }
  public bool? IsProficient { get; private set; }
  public Skill? Skill { get; private set; }
  public int? RemainingCharges { get; private set; }
  public int? RemainingResistance { get; private set; }
  public string? NameOverride { get; private set; }
  public string? DescriptionOverride { get; private set; }
  public double? ValueOverride { get; private set; }

  public InventoryEntity(CharacterEntity character, ItemEntity item, Character.InventoryUpdatedEvent @event)
  {
    Id = @event.InventoryId;

    Character = character;
    CharacterId = character.CharacterId;
    Item = item;
    ItemId = item.ItemId;

    Update(@event);
  }

  private InventoryEntity()
  {
  }

  public void Update(Character.InventoryUpdatedEvent @event)
  {
    ContainingItemId = @event.Item.ContainingItemId;
    Quantity = @event.Item.Quantity;
    IsAttuned = @event.Item.IsAttuned;
    IsEquipped = @event.Item.IsEquipped;
    IsIdentified = @event.Item.IsIdentified;
    IsProficient = @event.Item.IsProficient;
    Skill = @event.Item.Skill;
    RemainingCharges = @event.Item.RemainingCharges;
    RemainingResistance = @event.Item.RemainingResistance;
    NameOverride = @event.Item.NameOverride?.Value;
    DescriptionOverride = @event.Item.DescriptionOverride?.Value;
    ValueOverride = @event.Item.ValueOverride;
  }

  public override bool Equals(object? obj) => obj is InventoryEntity entity && entity.Id == Id;
  public override int GetHashCode() => Id.GetHashCode();
  public override string ToString() => $"{GetType()} (Id={Id})";
}
