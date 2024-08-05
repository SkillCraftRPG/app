using Logitar.EventSourcing;
using MediatR;

namespace SkillCraft.Domain.Storage.Events;

public class StorageInitializedEvent : DomainEvent, INotification
{
  public Guid UserId { get; }

  public long AllocatedBytes { get; }

  public StorageInitializedEvent(Guid userId, long allocatedBytes)
  {
    UserId = userId;
    AllocatedBytes = allocatedBytes;
  }
}
