using SkillCraft.Contracts;
using SkillCraft.Domain;
using SkillCraft.Domain.Characters;

namespace SkillCraft.Application.Characters;

internal static class CharacterExtensions
{
  public static EntityMetadata GetMetadata(this Character character) => new(character.WorldId, new EntityKey(EntityType.Character, character.EntityId), character.CalculateSize());

  private static long CalculateSize(this Character character) => character.Name.Size + (character.Player?.Size ?? 0); // TODO(fpion): calculate character size
}
