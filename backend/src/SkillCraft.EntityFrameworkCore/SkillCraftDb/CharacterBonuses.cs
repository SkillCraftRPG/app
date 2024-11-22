using Logitar.Data;
using SkillCraft.EntityFrameworkCore.Entities;

namespace SkillCraft.EntityFrameworkCore.SkillCraftDb;

internal static class CharacterBonuses
{
  public static readonly TableId Table = new(nameof(SkillCraftContext.CharacterBonuses));

  public static readonly ColumnId Category = new(nameof(CharacterBonusEntity.Category), Table);
  public static readonly ColumnId CharacterBonusId = new(nameof(CharacterBonusEntity.CharacterBonusId), Table);
  public static readonly ColumnId CharacterId = new(nameof(CharacterBonusEntity.CharacterId), Table);
  public static readonly ColumnId Id = new(nameof(CharacterBonusEntity.Id), Table);
  public static readonly ColumnId IsTemporary = new(nameof(CharacterBonusEntity.IsTemporary), Table);
  public static readonly ColumnId Notes = new(nameof(CharacterBonusEntity.Notes), Table);
  public static readonly ColumnId Precision = new(nameof(CharacterBonusEntity.Precision), Table);
  public static readonly ColumnId Target = new(nameof(CharacterBonusEntity.Target), Table);
  public static readonly ColumnId Value = new(nameof(CharacterBonusEntity.Value), Table);
}
