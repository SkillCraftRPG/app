using Logitar.Data;
using SkillCraft.EntityFrameworkCore.Entities;

namespace SkillCraft.EntityFrameworkCore.SkillCraftDb;

internal static class Lineages
{
  public static readonly TableId Table = new(nameof(SkillCraftContext.Lineages));

  public static readonly ColumnId AggregateId = new(nameof(LineageEntity.AggregateId), Table);
  public static readonly ColumnId CreatedBy = new(nameof(LineageEntity.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(LineageEntity.CreatedOn), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(LineageEntity.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(LineageEntity.UpdatedOn), Table);
  public static readonly ColumnId Version = new(nameof(LineageEntity.Version), Table);

  public static readonly ColumnId Description = new(nameof(LineageEntity.Description), Table);
  public static readonly ColumnId Id = new(nameof(LineageEntity.Id), Table);
  public static readonly ColumnId LineageId = new(nameof(LineageEntity.LineageId), Table);
  public static readonly ColumnId Name = new(nameof(LineageEntity.Name), Table);
  public static readonly ColumnId ParentId = new(nameof(LineageEntity.ParentId), Table);
  public static readonly ColumnId WorldId = new(nameof(LineageEntity.WorldId), Table);
}
