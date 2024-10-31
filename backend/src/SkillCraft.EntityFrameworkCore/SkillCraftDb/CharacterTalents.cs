using Logitar.Data;
using SkillCraft.EntityFrameworkCore.Entities;

namespace SkillCraft.EntityFrameworkCore.SkillCraftDb;

internal static class CharacterTalents
{
  public static readonly TableId Table = new(nameof(SkillCraftContext.CharacterTalents));

  public static readonly ColumnId CharacterId = new(nameof(CharacterTalentEntity.CharacterId), Table);
  public static readonly ColumnId CharacterTalentId = new(nameof(CharacterTalentEntity.CharacterTalentId), Table);
  public static readonly ColumnId Cost = new(nameof(CharacterTalentEntity.Cost), Table);
  public static readonly ColumnId Id = new(nameof(CharacterTalentEntity.Id), Table);
  public static readonly ColumnId Notes = new(nameof(CharacterTalentEntity.Notes), Table);
  public static readonly ColumnId Precision = new(nameof(CharacterTalentEntity.Precision), Table);
  public static readonly ColumnId TalentId = new(nameof(CharacterTalentEntity.TalentId), Table);
}
