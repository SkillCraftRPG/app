using SkillCraft.Contracts;
using SkillCraft.Contracts.Personalities;
using SkillCraft.Domain;
using SkillCraft.Domain.Personalities;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Personalities;

internal static class PersonalityExtensions
{
  private const EntityType Type = EntityType.Personality;

  public static EntityMetadata GetMetadata(this Personality personality)
  {
    long size = personality.Name.Size + (personality.Description?.Size ?? 0) + 4 + 4;
    return new EntityMetadata(personality.WorldId, new EntityKey(Type, personality.Id.ToGuid()), size);
  }
  public static EntityMetadata GetMetadata(this PersonalityModel personality)
  {
    long size = personality.Name.Length + (personality.Description?.Length ?? 0) + 4 + 4;
    return new EntityMetadata(new WorldId(personality.World.Id), new EntityKey(Type, personality.Id), size);
  }
}
