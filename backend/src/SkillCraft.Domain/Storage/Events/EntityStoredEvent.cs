using Logitar.EventSourcing;
using MediatR;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Domain.Storage.Events;

public class EntityStoredEvent : DomainEvent, INotification
{
  public WorldId WorldId { get; }

  public EntityType EntityType { get; }
  public Guid EntityId { get; }

  public long Size { get; }

  public long UsedBytes { get; }

  public EntityStoredEvent(WorldId worldId, EntityType entityType, Guid entityId, long size, long usedBytes)
  {
    WorldId = worldId;

    EntityType = entityType;
    EntityId = entityId;

    Size = size;

    UsedBytes = usedBytes;
  }

  public static EntityStoredEvent From(IStoredEntity entity, long usedBytes) => new(entity.WorldId, entity.EntityType, entity.EntityId, entity.Size, usedBytes);
}
