using Logitar.Data;
using SkillCraft.EntityFrameworkCore.Entities;

namespace SkillCraft.EntityFrameworkCore.SkillCraftDb;

internal static class Items
{
  public static readonly TableId Table = new(nameof(SkillCraftContext.Items));

  public static readonly ColumnId AggregateId = new(nameof(ItemEntity.AggregateId), Table);
  public static readonly ColumnId CreatedBy = new(nameof(ItemEntity.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(ItemEntity.CreatedOn), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(ItemEntity.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(ItemEntity.UpdatedOn), Table);
  public static readonly ColumnId Version = new(nameof(ItemEntity.Version), Table);

  public static readonly ColumnId Category = new(nameof(ItemEntity.Category), Table);
  public static readonly ColumnId Description = new(nameof(ItemEntity.Description), Table);
  public static readonly ColumnId Id = new(nameof(ItemEntity.Id), Table);
  public static readonly ColumnId ItemId = new(nameof(ItemEntity.ItemId), Table);
  public static readonly ColumnId Name = new(nameof(ItemEntity.Name), Table);
  public static readonly ColumnId Properties = new(nameof(ItemEntity.Properties), Table);
  public static readonly ColumnId Value = new(nameof(ItemEntity.Value), Table);
  public static readonly ColumnId Weight = new(nameof(ItemEntity.Weight), Table);
  public static readonly ColumnId WorldId = new(nameof(ItemEntity.WorldId), Table);
}
