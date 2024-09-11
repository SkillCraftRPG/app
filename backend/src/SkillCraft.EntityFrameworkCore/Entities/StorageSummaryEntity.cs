using SkillCraft.Domain.Storages;

namespace SkillCraft.EntityFrameworkCore.Entities;

internal class StorageSummaryEntity : AggregateEntity
{
  public UserEntity? Owner { get; private set; }
  public int UserId { get; private set; }
  public Guid OwnerId { get; private set; }

  public long AllocatedBytes { get; private set; }
  public long UsedBytes { get; private set; }
  public long AvailableBytes
  {
    get => AllocatedBytes - UsedBytes;
    private set { }
  }

  public StorageSummaryEntity(UserEntity owner, Storage.InitializedEvent @event) : base(@event)
  {
    Owner = owner;
    UserId = owner.UserId;
    OwnerId = owner.Id;

    AllocatedBytes = @event.AllocatedBytes;
  }

  private StorageSummaryEntity() : base()
  {
  }

  public void Store(Storage.EntityStoredEvent @event)
  {
    base.Update(@event);

    UsedBytes = @event.UsedBytes;
  }

  // TODO(fpion): migration
}
