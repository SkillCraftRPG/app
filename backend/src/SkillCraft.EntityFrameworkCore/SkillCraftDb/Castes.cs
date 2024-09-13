using Logitar.Data;
using SkillCraft.EntityFrameworkCore.Entities;

namespace SkillCraft.EntityFrameworkCore.SkillCraftDb;

internal static class Castes
{
  public static readonly TableId Table = new(nameof(SkillCraftContext.Castes));

  public static readonly ColumnId AggregateId = new(nameof(CasteEntity.AggregateId), Table);
  public static readonly ColumnId CreatedBy = new(nameof(CasteEntity.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(CasteEntity.CreatedOn), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(CasteEntity.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(CasteEntity.UpdatedOn), Table);
  public static readonly ColumnId Version = new(nameof(CasteEntity.Version), Table);

  public static readonly ColumnId CasteId = new(nameof(CasteEntity.CasteId), Table);
  public static readonly ColumnId Description = new(nameof(CasteEntity.Description), Table);
  public static readonly ColumnId Id = new(nameof(CasteEntity.Id), Table);
  public static readonly ColumnId Name = new(nameof(CasteEntity.Name), Table);
  public static readonly ColumnId Skill = new(nameof(CasteEntity.Skill), Table);
  public static readonly ColumnId Traits = new(nameof(CasteEntity.Traits), Table);
  public static readonly ColumnId WealthRoll = new(nameof(CasteEntity.WealthRoll), Table);
  public static readonly ColumnId WorldId = new(nameof(CasteEntity.WorldId), Table);
}
