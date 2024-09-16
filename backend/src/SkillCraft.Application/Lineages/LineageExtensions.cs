using SkillCraft.Contracts.Lineages;
using SkillCraft.Domain;
using SkillCraft.Domain.Lineages;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Lineages;

internal static class LineageExtensions
{
  private const EntityType Type = EntityType.Lineage;

  public static EntityMetadata GetMetadata(this Lineage lineage)
  {
    long size = 4 + lineage.Name.Size + (lineage.Description?.Size ?? 0) + 32;
    return new EntityMetadata(lineage.WorldId, new EntityKey(Type, lineage.Id.ToGuid()), size);
  }
  public static EntityMetadata GetMetadata(this LineageModel lineage)
  {
    long size = 4 + lineage.Name.Length + (lineage.Description?.Length ?? 0) + 32;
    return new EntityMetadata(new WorldId(lineage.World.Id), new EntityKey(Type, lineage.Id), size);
  }
}
