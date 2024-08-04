using Logitar.EventSourcing;
using Logitar.Identity.Contracts;
using Logitar.Identity.Domain.Shared;
using MediatR;

namespace SkillCraft.Domain.Worlds.Events;

public class WorldUpdatedEvent : DomainEvent, INotification
{
  public SlugUnit? UniqueSlug { get; set; }
  public Modification<DisplayNameUnit>? DisplayName { get; set; }
  public Modification<DescriptionUnit>? Description { get; set; }

  public bool HasChanges => UniqueSlug != null || DisplayName != null || Description != null;
}
