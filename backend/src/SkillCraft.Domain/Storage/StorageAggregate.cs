using Logitar.EventSourcing;
using SkillCraft.Domain.Storage.Events;

namespace SkillCraft.Domain.Storage;

public class StorageAggregate : AggregateRoot // TODO(fpion): unit tests
{
  public Guid UserId { get; private set; }

  public long AllocatedBytes { get; private set; }
  public long UsedBytes => _storedEntities.Values.Sum(e => e.Size);
  public long AvailableBytes => AllocatedBytes - UsedBytes;

  private readonly Dictionary<string, StoredEntity> _storedEntities = [];

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
      throw new ArgumentException("The allocated bytes shall be a positive integer value.", nameof(allocatedBytes));
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
    if (!_storedEntities.TryGetValue(key, out StoredEntity? existing) || existing.WorldId != entity.WorldId || existing.Size != entity.Size)
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
