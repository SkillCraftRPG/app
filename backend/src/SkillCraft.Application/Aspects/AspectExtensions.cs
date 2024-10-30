using SkillCraft.Contracts;
using SkillCraft.Domain;
using SkillCraft.Domain.Aspects;

namespace SkillCraft.Application.Aspects;

internal static class AspectExtensions
{
  public static EntityMetadata GetMetadata(this Aspect aspect) => new(aspect.WorldId, new EntityKey(EntityType.Aspect, aspect.EntityId), aspect.CalculateSize());

  private static long CalculateSize(this Aspect aspect) => aspect.Name.Size + (aspect.Description?.Size ?? 0)
    + 16 /* Attributes */ + 8 /* Skills */;
}
