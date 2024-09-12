using Logitar.Data;
using SkillCraft.EntityFrameworkCore.Entities;

namespace SkillCraft.EntityFrameworkCore.SkillCraftDb;

internal static class Educations
{
  public static readonly TableId Table = new(nameof(SkillCraftContext.Educations));

  public static readonly ColumnId AggregateId = new(nameof(EducationEntity.AggregateId), Table);
  public static readonly ColumnId CreatedBy = new(nameof(EducationEntity.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(EducationEntity.CreatedOn), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(EducationEntity.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(EducationEntity.UpdatedOn), Table);
  public static readonly ColumnId Version = new(nameof(EducationEntity.Version), Table);

  public static readonly ColumnId Description = new(nameof(EducationEntity.Description), Table);
  public static readonly ColumnId EducationId = new(nameof(EducationEntity.EducationId), Table);
  public static readonly ColumnId Id = new(nameof(EducationEntity.Id), Table);
  public static readonly ColumnId Name = new(nameof(EducationEntity.Name), Table);
  public static readonly ColumnId Skill = new(nameof(EducationEntity.Skill), Table);
  public static readonly ColumnId WealthMultiplier = new(nameof(EducationEntity.WealthMultiplier), Table);
  public static readonly ColumnId WorldId = new(nameof(EducationEntity.WorldId), Table);
}
