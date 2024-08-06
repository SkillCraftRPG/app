using SkillCraft.Domain.Storage.Events;

namespace SkillCraft.EntityFrameworkCore.Entities;

internal class StorageSummaryEntity
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

  public StorageSummaryEntity(StorageInitializedEvent @event)
  {
    UserId = @event.UserId;

    AllocatedBytes = @event.AllocatedBytes;
  }

  private StorageSummaryEntity()
  {
  }

  public void Update(EntityStoredEvent @event)
  {
    UsedBytes = @event.UsedBytes;
  }
}
