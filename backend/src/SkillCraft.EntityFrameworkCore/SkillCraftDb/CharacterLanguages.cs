using Logitar.Data;
using SkillCraft.EntityFrameworkCore.Entities;

namespace SkillCraft.EntityFrameworkCore.SkillCraftDb;

internal static class CharacterLanguages
{
  public static readonly TableId Table = new(nameof(SkillCraftContext.CharacterLanguages));

  public static readonly ColumnId CharacterId = new(nameof(CharacterLanguageEntity.CharacterId), Table);
  public static readonly ColumnId LanguageId = new(nameof(CharacterLanguageEntity.LanguageId), Table);
  public static readonly ColumnId Notes = new(nameof(CharacterLanguageEntity.Notes), Table);
}
