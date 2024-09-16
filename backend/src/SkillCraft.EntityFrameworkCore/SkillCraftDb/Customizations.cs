using Logitar.Data;
using SkillCraft.EntityFrameworkCore.Entities;

namespace SkillCraft.EntityFrameworkCore.SkillCraftDb;

internal static class Customizations
{
  public static readonly TableId Table = new(nameof(SkillCraftContext.Customizations));

  public static readonly ColumnId AggregateId = new(nameof(CustomizationEntity.AggregateId), Table);
  public static readonly ColumnId CreatedBy = new(nameof(CustomizationEntity.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(CustomizationEntity.CreatedOn), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(CustomizationEntity.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(CustomizationEntity.UpdatedOn), Table);
  public static readonly ColumnId Version = new(nameof(CustomizationEntity.Version), Table);

  public static readonly ColumnId CustomizationId = new(nameof(CustomizationEntity.CustomizationId), Table);
  public static readonly ColumnId Description = new(nameof(CustomizationEntity.Description), Table);
  public static readonly ColumnId Id = new(nameof(CustomizationEntity.Id), Table);
  public static readonly ColumnId Name = new(nameof(CustomizationEntity.Name), Table);
  public static readonly ColumnId Type = new(nameof(CustomizationEntity.Type), Table);
  public static readonly ColumnId WorldId = new(nameof(CustomizationEntity.WorldId), Table);
}
