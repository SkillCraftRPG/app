﻿using SkillCraft.Domain.Worlds;

namespace SkillCraft.Domain.Storage;

public class StoredEntityMock : IStoredEntity
{
  public WorldId WorldId { get; }

  public EntityType EntityType { get; }
  public Guid EntityId { get; }

  public int Size { get; }

  public StoredEntityMock(WorldId worldId, EntityType entityType, Guid entityId, int size)
  {
    WorldId = worldId;

    EntityType = entityType;
    EntityId = entityId;

    Size = size;
  }
}
