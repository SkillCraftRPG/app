using SkillCraft.Contracts;
using SkillCraft.Domain;
using SkillCraft.Domain.Characters;

namespace SkillCraft.Application.Characters;

internal static class CharacterExtensions
{
  public static EntityMetadata GetMetadata(this Character character)
  {
    long size = character.Name.Size + (character.Player?.Size ?? 0)
      + 4 /* LineageId */ + 8 /* Height */ + 8 /* Weight */ + 4 /* Age */;
    // TODO(fpion): complete
    return new EntityMetadata(character.WorldId, new EntityKey(EntityType.Character, character.EntityId), size);
  }
}
