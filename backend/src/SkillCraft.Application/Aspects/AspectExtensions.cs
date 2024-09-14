using SkillCraft.Contracts.Aspects;
using SkillCraft.Domain;
using SkillCraft.Domain.Aspects;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Aspects;

internal static class AspectExtensions
{
  private const EntityType Type = EntityType.Aspect;

  public static EntityMetadata GetMetadata(this Aspect aspect)
  {
    long size = aspect.Name.Size + (aspect.Description?.Size ?? 0) + 16 + 8;
    return new EntityMetadata(aspect.WorldId, new EntityKey(Type, aspect.Id.ToGuid()), size);
  }
  public static EntityMetadata GetMetadata(this AspectModel aspect)
  {
    long size = aspect.Name.Length + (aspect.Description?.Length ?? 0) + 16 + 8;
    return new EntityMetadata(new WorldId(aspect.World.Id), new EntityKey(Type, aspect.Id), size);
  }
}
