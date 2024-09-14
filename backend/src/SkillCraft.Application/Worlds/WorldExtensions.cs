using SkillCraft.Contracts.Worlds;
using SkillCraft.Domain;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Worlds;

internal static class WorldExtensions
{
  private const EntityType Type = EntityType.World;

  public static EntityMetadata GetMetadata(this World world)
  {
    long size = world.Slug.Size + (world.Name?.Size ?? 0) + (world.Description?.Size ?? 0);
    return new EntityMetadata(world.Id, new EntityKey(Type, world.Id.ToGuid()), size);
  }
  public static EntityMetadata GetMetadata(this WorldModel world)
  {
    long size = world.Slug.Length + (world.Name?.Length ?? 0) + (world.Description?.Length ?? 0);
    return new EntityMetadata(new WorldId(world.Id), new EntityKey(Type, world.Id), size);
  }
}
