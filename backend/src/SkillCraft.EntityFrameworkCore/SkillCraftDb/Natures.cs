using Logitar.Data;
using SkillCraft.EntityFrameworkCore.Entities;

namespace SkillCraft.EntityFrameworkCore.SkillCraftDb;

internal static class Natures
{
  public static readonly TableId Table = new(nameof(SkillCraftContext.Natures));

  public static readonly ColumnId AggregateId = new(nameof(NatureEntity.AggregateId), Table);
  public static readonly ColumnId CreatedBy = new(nameof(NatureEntity.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(NatureEntity.CreatedOn), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(NatureEntity.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(NatureEntity.UpdatedOn), Table);
  public static readonly ColumnId Version = new(nameof(NatureEntity.Version), Table);

  public static readonly ColumnId Attribute = new(nameof(NatureEntity.Attribute), Table);
  public static readonly ColumnId Description = new(nameof(NatureEntity.Description), Table);
  public static readonly ColumnId GiftId = new(nameof(NatureEntity.GiftId), Table);
  public static readonly ColumnId Id = new(nameof(NatureEntity.Id), Table);
  public static readonly ColumnId Name = new(nameof(NatureEntity.Name), Table);
  public static readonly ColumnId NatureId = new(nameof(NatureEntity.NatureId), Table);
  public static readonly ColumnId WorldId = new(nameof(NatureEntity.WorldId), Table);
}
