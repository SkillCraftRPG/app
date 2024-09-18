using Logitar.Data;
using SkillCraft.EntityFrameworkCore.Entities;

namespace SkillCraft.EntityFrameworkCore.SkillCraftDb;

internal static class LineageLanguages
{
  public static readonly TableId Table = new(nameof(SkillCraftContext.LineageLanguages));

  public static readonly ColumnId LanguageId = new(nameof(LineageLanguageEntity.LanguageId), Table);
  public static readonly ColumnId LineageId = new(nameof(LineageLanguageEntity.LineageId), Table);
}
