using SkillCraft.Domain;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application;

public record EntityMetadata
{
  public WorldId WorldId { get; }
  public EntityKey Key { get; }
  public long Size { get; }

  public EntityType Type => Key.Type;
  public Guid Id => Key.Id;

  public EntityMetadata(WorldId worldId, EntityKey key, long size)
  {
    ArgumentOutOfRangeException.ThrowIfNegativeOrZero(size, nameof(size));

    WorldId = worldId;
    Key = key;
    Size = size;
  }
}
