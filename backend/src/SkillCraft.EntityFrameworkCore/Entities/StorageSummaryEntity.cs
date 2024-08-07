using SkillCraft.Domain.Storage.Events;

namespace SkillCraft.EntityFrameworkCore.Entities;

internal class StorageSummaryEntity : AggregateEntity
{
  public int StorageSummaryId { get; private set; }

  public Guid UserId { get; private set; }

  public long AllocatedBytes { get; private set; }
  public long UsedBytes { get; private set; }
  public long AvailableBytes
  {
    get => AllocatedBytes - UsedBytes;
    private set { }
  }

  private StorageSummaryEntity() : base()
  {
  }

  public StorageSummaryEntity(StorageInitializedEvent @event) : base(@event)
  {
    UserId = @event.UserId;

    AllocatedBytes = @event.AllocatedBytes;
  }

  public void Update(EntityStoredEvent @event)
  {
    base.Update(@event);

    UsedBytes = @event.UsedBytes;
  }
}
