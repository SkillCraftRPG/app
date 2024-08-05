using Logitar.EventSourcing;
using SkillCraft.Domain.Storage.Events;

namespace SkillCraft.Domain.Storage;

public class StorageAggregate : AggregateRoot
{
  public Guid UserId { get; private set; }

  public long AllocatedBytes { get; private set; }
  public long UsedBytes => 0; // TODO(fpion): computed
  public long AvailableBytes => AllocatedBytes - UsedBytes;

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
}
