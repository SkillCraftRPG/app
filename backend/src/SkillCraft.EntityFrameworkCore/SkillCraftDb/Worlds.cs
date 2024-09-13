using Logitar.Data;
using SkillCraft.EntityFrameworkCore.Entities;

namespace SkillCraft.EntityFrameworkCore.SkillCraftDb;

internal static class Worlds
{
  public static readonly TableId Table = new(nameof(SkillCraftContext.Worlds));

  public static readonly ColumnId AggregateId = new(nameof(WorldEntity.AggregateId), Table);
  public static readonly ColumnId CreatedBy = new(nameof(WorldEntity.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(WorldEntity.CreatedOn), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(WorldEntity.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(WorldEntity.UpdatedOn), Table);
  public static readonly ColumnId Version = new(nameof(WorldEntity.Version), Table);

  public static readonly ColumnId Description = new(nameof(WorldEntity.Description), Table);
  public static readonly ColumnId Id = new(nameof(WorldEntity.Id), Table);
  public static readonly ColumnId Name = new(nameof(WorldEntity.Name), Table);
  public static readonly ColumnId OwnerId = new(nameof(WorldEntity.OwnerId), Table);
  public static readonly ColumnId Slug = new(nameof(WorldEntity.Slug), Table);
  public static readonly ColumnId SlugNormalized = new(nameof(WorldEntity.SlugNormalized), Table);
  public static readonly ColumnId UserId = new(nameof(WorldEntity.UserId), Table);
  public static readonly ColumnId WorldId = new(nameof(WorldEntity.WorldId), Table);
}
