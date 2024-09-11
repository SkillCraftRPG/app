using Logitar.Data;
using SkillCraft.EntityFrameworkCore.Entities;

namespace SkillCraft.EntityFrameworkCore.SkillCraftDb;

internal static class StorageDetails
{
  public static readonly TableId Table = new(nameof(SkillCraftContext.StorageDetails));

  public static readonly ColumnId EntityId = new(nameof(StorageDetailEntity.EntityId), Table);
  public static readonly ColumnId EntityType = new(nameof(StorageDetailEntity.EntityType), Table);
  public static readonly ColumnId OwnerId = new(nameof(StorageDetailEntity.OwnerId), Table);
  public static readonly ColumnId Size = new(nameof(StorageDetailEntity.Size), Table);
  public static readonly ColumnId StorageDetailId = new(nameof(StorageDetailEntity.StorageDetailId), Table);
  public static readonly ColumnId UserId = new(nameof(StorageDetailEntity.UserId), Table);
  public static readonly ColumnId WorldId = new(nameof(StorageDetailEntity.WorldId), Table);
}
