using Logitar.Data;
using SkillCraft.EntityFrameworkCore.Entities;

namespace SkillCraft.EntityFrameworkCore.SkillCraftDb;

internal static class Inventory
{
  public static readonly TableId Table = new(nameof(SkillCraftContext.Inventory));

  public static readonly ColumnId CharacterId = new(nameof(InventoryEntity.CharacterId), Table);
  public static readonly ColumnId ContainingItemId = new(nameof(InventoryEntity.ContainingItemId), Table);
  public static readonly ColumnId DescriptionOverride = new(nameof(InventoryEntity.DescriptionOverride), Table);
  public static readonly ColumnId Id = new(nameof(InventoryEntity.Id), Table);
  public static readonly ColumnId InventoryId = new(nameof(InventoryEntity.InventoryId), Table);
  public static readonly ColumnId IsAttuned = new(nameof(InventoryEntity.IsAttuned), Table);
  public static readonly ColumnId IsEquipped = new(nameof(InventoryEntity.IsEquipped), Table);
  public static readonly ColumnId IsIdentified = new(nameof(InventoryEntity.IsIdentified), Table);
  public static readonly ColumnId IsProficient = new(nameof(InventoryEntity.IsProficient), Table);
  public static readonly ColumnId ItemId = new(nameof(InventoryEntity.ItemId), Table);
  public static readonly ColumnId NameOverride = new(nameof(InventoryEntity.NameOverride), Table);
  public static readonly ColumnId Quantity = new(nameof(InventoryEntity.Quantity), Table);
  public static readonly ColumnId RemainingCharges = new(nameof(InventoryEntity.RemainingCharges), Table);
  public static readonly ColumnId RemainingResistance = new(nameof(InventoryEntity.RemainingResistance), Table);
  public static readonly ColumnId Skill = new(nameof(InventoryEntity.Skill), Table);
  public static readonly ColumnId ValueOverride = new(nameof(InventoryEntity.ValueOverride), Table);
}
