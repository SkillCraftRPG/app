using SkillCraft.Contracts;
using SkillCraft.Domain;
using SkillCraft.Domain.Personalities;

namespace SkillCraft.Application.Personalities;

internal static class PersonalityExtensions
{
  public static EntityMetadata GetMetadata(this Personality personality) => new(personality.WorldId, new EntityKey(EntityType.Personality, personality.EntityId), personality.CalculateSize());

  private static long CalculateSize(this Personality personality) => personality.Name.Size + (personality.Description?.Size ?? 0)
    + 4 /* Attribute */ + 4 /* GiftId */;
}
