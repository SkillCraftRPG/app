using Logitar.EventSourcing;
using MediatR;

namespace SkillCraft.Domain.Worlds.Events;

public class WorldDeletedEvent : DomainEvent, INotification
{
  public WorldDeletedEvent()
  {
    IsDeleted = true;
  }
}
