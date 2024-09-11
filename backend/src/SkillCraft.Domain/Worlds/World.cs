using Logitar.EventSourcing;
using MediatR;

namespace SkillCraft.Domain.Worlds;

public class World : AggregateRoot
{
  private UpdatedEvent _updatedEvent = new();

  public new WorldId Id => new(base.Id);

  public UserId OwnerId { get; private set; }

  private Slug? _slug = null;
  public Slug Slug
  {
    get => _slug ?? throw new InvalidOperationException($"The {nameof(Slug)} has not been initialized yet.");
    set
    {
      if (_slug != value)
      {
        _slug = value;
        _updatedEvent.Slug = value;
      }
    }
  }
  private Name? _name = null;
  public Name? Name
  {
    get => _name;
    set
    {
      if (_name != value)
      {
        _name = value;
        _updatedEvent.Name = new Change<Name>(value);
      }
    }
  }
  private Description? _description = null;
  public Description? Description
  {
    get => _description;
    set
    {
      if (_description != value)
      {
        _description = value;
        _updatedEvent.Description = new Change<Description>(value);
      }
    }
  }

  public World() : base()
  {
  }

  public World(Slug slug, UserId ownerId, WorldId? id = null) : base(id?.AggregateId)
  {
    Raise(new CreatedEvent(ownerId, slug), ownerId.ActorId);
  }
  protected virtual void Apply(CreatedEvent @event)
  {
    OwnerId = @event.OwnerId;

    _slug = @event.Slug;
  }

  public void Update(UserId userId)
  {
    if (_updatedEvent.HasChanges)
    {
      Raise(_updatedEvent, userId.ActorId, DateTime.Now);
      _updatedEvent = new();
    }
  }
  protected virtual void Apply(UpdatedEvent @event)
  {
    if (@event.Slug != null)
    {
      _slug = @event.Slug;
    }
    if (@event.Name != null)
    {
      _name = @event.Name.Value;
    }
    if (@event.Description != null)
    {
      _description = @event.Description.Value;
    }
  }

  public override string ToString() => $"{Name?.Value ?? Slug.Value} | {base.ToString()}";

  public class CreatedEvent : DomainEvent, INotification
  {
    public UserId OwnerId { get; }

    public Slug Slug { get; }

    public CreatedEvent(UserId ownerId, Slug slug)
    {
      OwnerId = ownerId;

      Slug = slug;
    }
  }

  public class UpdatedEvent : DomainEvent, INotification
  {
    public Slug? Slug { get; set; }
    public Change<Name>? Name { get; set; }
    public Change<Description>? Description { get; set; }

    public bool HasChanges => Slug != null || Name != null || Description != null;
  }
}
