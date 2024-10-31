using Logitar.Data;
using SkillCraft.EntityFrameworkCore.Entities;

namespace SkillCraft.EntityFrameworkCore.SkillCraftDb;

internal static class CharacterCustomizations
{
  public static readonly TableId Table = new(nameof(SkillCraftContext.CharacterCustomizations));

  public static readonly ColumnId CharacterId = new(nameof(CharacterCustomizationEntity.CharacterId), Table);
  public static readonly ColumnId CustomizationId = new(nameof(CharacterCustomizationEntity.CustomizationId), Table);
}
