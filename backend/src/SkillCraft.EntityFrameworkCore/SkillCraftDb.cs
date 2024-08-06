using Logitar.Data;
using SkillCraft.EntityFrameworkCore.Entities;

namespace SkillCraft.EntityFrameworkCore;

internal static class SkillCraftDb
{
  public static class Actors
  {
    public static readonly TableId Table = new(nameof(SkillCraftContext.Actors));

    public static readonly ColumnId ActorId = new(nameof(ActorEntity.ActorId), Table);
    public static readonly ColumnId DisplayName = new(nameof(ActorEntity.DisplayName), Table);
    public static readonly ColumnId EmailAddress = new(nameof(ActorEntity.EmailAddress), Table);
    public static readonly ColumnId Id = new(nameof(ActorEntity.Id), Table);
    public static readonly ColumnId IsDeleted = new(nameof(ActorEntity.IsDeleted), Table);
    public static readonly ColumnId PictureUrl = new(nameof(ActorEntity.PictureUrl), Table);
    public static readonly ColumnId Type = new(nameof(ActorEntity.Type), Table);
  }

  public static class StorageDetails
  {
    public static readonly TableId Table = new(nameof(SkillCraftContext.StorageDetails));

    public static readonly ColumnId EntityId = new(nameof(StorageDetailEntity.EntityId), Table);
    public static readonly ColumnId EntityType = new(nameof(StorageDetailEntity.EntityType), Table);
    public static readonly ColumnId Size = new(nameof(StorageDetailEntity.Size), Table);
    public static readonly ColumnId StorageDetailId = new(nameof(StorageDetailEntity.StorageDetailId), Table);
    public static readonly ColumnId UserId = new(nameof(StorageDetailEntity.UserId), Table);
    public static readonly ColumnId WorldId = new(nameof(StorageDetailEntity.WorldId), Table);
  }

  public static class StorageSummaries
  {
    public static readonly TableId Table = new(nameof(SkillCraftContext.StorageSummaries));

    public static readonly ColumnId AllocatedBytes = new(nameof(StorageSummaryEntity.AllocatedBytes), Table);
    public static readonly ColumnId AvailableBytes = new(nameof(StorageSummaryEntity.AvailableBytes), Table);
    public static readonly ColumnId StorageSummaryId = new(nameof(StorageSummaryEntity.StorageSummaryId), Table);
    public static readonly ColumnId UsedBytes = new(nameof(StorageSummaryEntity.UsedBytes), Table);
    public static readonly ColumnId UserId = new(nameof(StorageSummaryEntity.UserId), Table);
  }

  public static class Worlds
  {
    public static readonly TableId Table = new(nameof(SkillCraftContext.Worlds));

    public static readonly ColumnId AggregateId = new(nameof(WorldEntity.AggregateId), Table);
    public static readonly ColumnId CreatedBy = new(nameof(WorldEntity.CreatedBy), Table);
    public static readonly ColumnId CreatedOn = new(nameof(WorldEntity.CreatedOn), Table);
    public static readonly ColumnId UpdatedBy = new(nameof(WorldEntity.UpdatedBy), Table);
    public static readonly ColumnId UpdatedOn = new(nameof(WorldEntity.UpdatedOn), Table);
    public static readonly ColumnId Version = new(nameof(WorldEntity.Version), Table);

    public static readonly ColumnId Description = new(nameof(WorldEntity.Description), Table);
    public static readonly ColumnId DisplayName = new(nameof(WorldEntity.DisplayName), Table);
    public static readonly ColumnId Id = new(nameof(WorldEntity.Id), Table);
    public static readonly ColumnId OwnerId = new(nameof(WorldEntity.OwnerId), Table);
    public static readonly ColumnId UniqueSlug = new(nameof(WorldEntity.UniqueSlug), Table);
    public static readonly ColumnId UniqueSlugNormalized = new(nameof(WorldEntity.UniqueSlugNormalized), Table);
    public static readonly ColumnId WorldId = new(nameof(WorldEntity.WorldId), Table);
  }

  public static string Normalize(string value) => value.Trim().ToUpperInvariant();
}
