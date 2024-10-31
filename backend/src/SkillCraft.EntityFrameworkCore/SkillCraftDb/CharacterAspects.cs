using Logitar.Data;
using SkillCraft.EntityFrameworkCore.Entities;

namespace SkillCraft.EntityFrameworkCore.SkillCraftDb;

internal static class CharacterAspects
{
  public static readonly TableId Table = new(nameof(SkillCraftContext.CharacterAspects));

  public static readonly ColumnId AspectId = new(nameof(CharacterAspectEntity.AspectId), Table);
  public static readonly ColumnId CharacterId = new(nameof(CharacterAspectEntity.CharacterId), Table);
}
