using SkillCraft.Contracts;
using SkillCraft.Contracts.Talents;
using SkillCraft.Domain;
using SkillCraft.Domain.Talents;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Talents;

internal static class TalentExtensions
{
  private const EntityType Type = EntityType.Talent;

  public static EntityMetadata GetMetadata(this Talent talent)
  {
    long size = 4 + talent.Name.Size + (talent.Description?.Size ?? 0) + 1 + 4;
    return new EntityMetadata(talent.WorldId, new EntityKey(Type, talent.Id.ToGuid()), size);
  }
  public static EntityMetadata GetMetadata(this TalentModel talent)
  {
    long size = 4 + talent.Name.Length + (talent.Description?.Length ?? 0) + 1 + 4;
    return new EntityMetadata(new WorldId(talent.World.Id), new EntityKey(Type, talent.Id), size);
  }
}
