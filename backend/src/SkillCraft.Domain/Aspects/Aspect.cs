using Logitar.EventSourcing;
using MediatR;
using SkillCraft.Contracts;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Domain.Aspects;

public class Aspect : AggregateRoot
{
  private UpdatedEvent _updatedEvent = new();

  public new AspectId Id => new(base.Id);

  public WorldId WorldId { get; private set; }

  private Name? _name = null;
  public Name Name
  {
    get => _name ?? throw new InvalidOperationException($"The {nameof(Name)} has not been initialized yet.");
    set
    {
      if (_name != value)
      {
        _name = value;
        _updatedEvent.Name = value;
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

  private Attributes? _attributes = null;
  public Attributes Attributes
  {
    get => _attributes ?? throw new InvalidOperationException($"The {nameof(Attributes)} has not been initialized yet.");
    set
    {
      if (_attributes != value)
      {
        _attributes = value;
        _updatedEvent.Attributes = value;
      }
    }
  }
  private Skills? _skills = null;
  public Skills Skills
  {
    get => _skills ?? throw new InvalidOperationException($"The {nameof(Skills)} has not been initialized yet.");
    set
    {
      if (_skills != value)
      {
        _skills = value;
        _updatedEvent.Skills = value;
      }
    }
  }

  public Aspect() : base()
  {
  }

  public Aspect(WorldId worldId, Name name, UserId userId, AspectId? id = null) : base(id?.AggregateId)
  {
    Raise(new CreatedEvent(worldId, name, new Attributes(), new Skills()), userId.ActorId);
  }
  protected virtual void Apply(CreatedEvent @event)
  {
    WorldId = @event.WorldId;

    _name = @event.Name;

    _attributes = @event.Attributes;
    _skills = @event.Skills;
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
    if (@event.Name != null)
    {
      _name = @event.Name;
    }
    if (@event.Description != null)
    {
      _description = @event.Description.Value;
    }

    if (@event.Attributes != null)
    {
      _attributes = @event.Attributes;
    }
    if (@event.Skills != null)
    {
      _skills = @event.Skills;
    }
  }

  public override string ToString() => $"{Name.Value} | {base.ToString()}";

  public class CreatedEvent : DomainEvent, INotification
  {
    public WorldId WorldId { get; }

    public Name Name { get; }

    public Attributes Attributes { get; }
    public Skills Skills { get; }

    public CreatedEvent(WorldId worldId, Name name, Attributes attributes, Skills skills)
    {
      WorldId = worldId;

      Name = name;

      Attributes = attributes;
      Skills = skills;
    }
  }

  public class UpdatedEvent : DomainEvent, INotification
  {
    public Name? Name { get; set; }
    public Change<Description>? Description { get; set; }

    public Attributes? Attributes { get; set; }
    public Skills? Skills { get; set; }

    public bool HasChanges => Name != null || Description != null || Attributes != null || Skills != null;
  }
}
