using Logitar.EventSourcing;
using MediatR;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Domain.Storages;

public class Storage : AggregateRoot
{
  public UserId UserId { get; private set; }

  public long AllocatedBytes { get; private set; }
  public long UsedBytes => _storedEntities.Sum(x => x.Value.Size);
  public long AvailableBytes => AllocatedBytes - UsedBytes;

  private readonly Dictionary<EntityKey, StoredEntity> _storedEntities = [];

  public Storage() : base()
  {
  }

  private Storage(UserId userId, long allocatedBytes) : base(new AggregateId(userId.Value))
  {
    Raise(new InitializedEvent(userId, allocatedBytes), userId.ActorId);
  }
  protected virtual void Apply(InitializedEvent @event)
  {
    UserId = @event.UserId;

    AllocatedBytes = @event.AllocatedBytes;
  }

  public static Storage Initialize(UserId userId, long allocatedBytes)
  {
    ArgumentOutOfRangeException.ThrowIfNegative(allocatedBytes, nameof(allocatedBytes));

    return new Storage(userId, allocatedBytes);
  }

  public long GetSize(EntityKey key) => _storedEntities.TryGetValue(key, out StoredEntity? entity) ? entity.Size : 0;

  public void Store(EntityKey key, long size, WorldId worldId)
  {
    if (!_storedEntities.TryGetValue(key, out StoredEntity? entity) || entity.Size != size || entity.WorldId != worldId)
    {
      long usedBytes = UsedBytes - (entity?.Size ?? 0) + size;

      entity = new(size, worldId);
      Raise(new EntityStoredEvent(key, entity, usedBytes), UserId.ActorId);
    }
  }
  protected virtual void Apply(EntityStoredEvent @event)
  {
    _storedEntities[@event.Key] = @event.Entity;
  }

  public class EntityStoredEvent : DomainEvent, INotification
  {
    public EntityKey Key { get; }
    public StoredEntity Entity { get; }

    public long UsedBytes { get; }

    public EntityStoredEvent(EntityKey key, StoredEntity entity, long usedBytes)
    {
      Key = key;
      Entity = entity;

      UsedBytes = usedBytes;
    }
  }

  public class InitializedEvent : DomainEvent, INotification
  {
    public UserId UserId { get; }

    public long AllocatedBytes { get; }

    public InitializedEvent(UserId userId, long allocatedBytes)
    {
      UserId = userId;

      AllocatedBytes = allocatedBytes;
    }
  }
}
