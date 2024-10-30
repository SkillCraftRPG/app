using SkillCraft.Contracts;
using SkillCraft.Domain;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Worlds;

internal static class WorldExtensions
{
  public static EntityMetadata GetMetadata(this World world) => new(world.Id, new EntityKey(EntityType.World, world.Id.ToGuid()), world.CalculateSize());

  private static long CalculateSize(this World world) => world.Slug.Size + (world.Name?.Size ?? 0) + (world.Description?.Size ?? 0);
}
