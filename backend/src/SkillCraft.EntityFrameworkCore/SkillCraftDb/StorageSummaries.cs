using Logitar.Data;
using SkillCraft.EntityFrameworkCore.Entities;

namespace SkillCraft.EntityFrameworkCore.SkillCraftDb;

internal static class StorageSummaries
{
  public static readonly TableId Table = new(nameof(SkillCraftContext.StorageSummaries));

  public static readonly ColumnId AggregateId = new(nameof(StorageSummaryEntity.AggregateId), Table);
  public static readonly ColumnId CreatedBy = new(nameof(StorageSummaryEntity.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(StorageSummaryEntity.CreatedOn), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(StorageSummaryEntity.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(StorageSummaryEntity.UpdatedOn), Table);
  public static readonly ColumnId Version = new(nameof(StorageSummaryEntity.Version), Table);

  public static readonly ColumnId AllocatedBytes = new(nameof(StorageSummaryEntity.AllocatedBytes), Table);
  public static readonly ColumnId AvailableBytes = new(nameof(StorageSummaryEntity.AvailableBytes), Table);
  public static readonly ColumnId OwnerId = new(nameof(StorageSummaryEntity.OwnerId), Table);
  public static readonly ColumnId UsedBytes = new(nameof(StorageSummaryEntity.UsedBytes), Table);
  public static readonly ColumnId UserId = new(nameof(StorageSummaryEntity.UserId), Table);
}
