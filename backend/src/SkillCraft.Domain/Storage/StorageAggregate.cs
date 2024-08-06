using Logitar.EventSourcing;
using SkillCraft.Domain.Storage.Events;

namespace SkillCraft.Domain.Storage;

public class StorageAggregate : AggregateRoot
{
  public Guid UserId { get; private set; }

  public long AllocatedBytes { get; private set; }
  public long UsedBytes => _storedEntities.Values.Sum(entity => entity.Size);
  public long AvailableBytes => AllocatedBytes - UsedBytes;

  private readonly Dictionary<string, StoredEntity> _storedEntities = [];
  public long GetSize(IStoredEntity entity) => GetSize(entity.EntityType, entity.EntityId);
  public long GetSize(EntityType entityType, Guid entityId)
  {
    string key = GetEntityKey(entityType, entityId);
    return _storedEntities.TryGetValue(key, out StoredEntity? entity) ? entity.Size : 0;
  }

  public StorageAggregate() : base()
  {
  }

  private StorageAggregate(AggregateId id) : base(id)
  {
  }

  public static StorageAggregate Initialize(Guid userId, long allocatedBytes)
  {
    if (userId == default)
    {
      throw new ArgumentException("The user identifier is required.", nameof(userId));
    }
    if (allocatedBytes < 1)
    {
      throw new ArgumentException("The allocated bytes must be a positive integer value.", nameof(allocatedBytes));
    }

    AggregateId aggregateId = new(userId);
    StorageAggregate storage = new(aggregateId);

    ActorId actorId = new(userId);
    storage.Raise(new StorageInitializedEvent(userId, allocatedBytes), actorId);

    return storage;
  }
  protected virtual void Apply(StorageInitializedEvent @event)
  {
    UserId = @event.UserId;

    AllocatedBytes = @event.AllocatedBytes;
  }

  public void Store(IStoredEntity entity)
  {
    string key = GetEntityKey(entity);
    if (!_storedEntities.TryGetValue(key, out StoredEntity? existing) || existing.Size != entity.Size)
    {
      long usedBytes = UsedBytes + entity.Size;
      if (existing != null)
      {
        usedBytes -= existing.Size;
      }

      Raise(EntityStoredEvent.From(entity, usedBytes), new ActorId(UserId));
    }
  }
  protected virtual void Apply(EntityStoredEvent @event)
  {
    string key = GetEntityKey(@event.EntityType, @event.EntityId);
    _storedEntities[key] = new StoredEntity(@event.WorldId, @event.Size);
  }
  private static string GetEntityKey(IStoredEntity entity) => GetEntityKey(entity.EntityType, entity.EntityId);
  private static string GetEntityKey(EntityType entityType, Guid entityId) => $"{entityType}.Id:{entityId}";
}
