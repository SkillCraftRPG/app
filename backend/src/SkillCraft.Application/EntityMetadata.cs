﻿using SkillCraft.Domain;
using SkillCraft.Domain.Storages;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application;

public class EntityMetadata
{
  public WorldId WorldId { get; }

  public EntityType Type { get; }
  public Guid Id { get; }
  public EntityKey StorageKey { get; }

  public long Size { get; }

  private EntityMetadata(WorldId worldId, EntityType type, Guid id, long size)
  {
    WorldId = worldId;

    Type = type;
    Id = id;
    StorageKey = new(type, id);

    Size = size;
  }

  public static EntityMetadata From(World world)
  {
    long size = world.Slug.Value.Length + (world.Name?.Value.Length ?? 0) + (world.Description?.Value.Length ?? 0);
    return new EntityMetadata(world.Id, EntityType.World, world.Id.ToGuid(), size);
  }
}
