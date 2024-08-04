using Logitar.EventSourcing;
using MediatR;

namespace SkillCraft.Domain.Worlds.Events;

public class WorldCreatedEvent : DomainEvent, INotification
{
  public SlugUnit UniqueSlug { get; }

  public WorldCreatedEvent(SlugUnit uniqueSlug)
  {
    UniqueSlug = uniqueSlug;
  }
}
