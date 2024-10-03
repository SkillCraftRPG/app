using Logitar.EventSourcing;
using MediatR;
using SkillCraft.Contracts;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Domain.Languages;

public class Language : AggregateRoot
{
  private UpdatedEvent _updatedEvent = new();

  public new LanguageId Id => new(base.Id);
  public WorldId WorldId => Id.WorldId;
  public Guid EntityId => Id.EntityId;

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

  private Script? _script = null;
  public Script? Script
  {
    get => _script;
    set
    {
      if (_script != value)
      {
        _script = value;
        _updatedEvent.Script = new Change<Script>(value);
      }
    }
  }
  private TypicalSpeakers? _typicalSpeakers = null;
  public TypicalSpeakers? TypicalSpeakers
  {
    get => _typicalSpeakers;
    set
    {
      if (_typicalSpeakers != value)
      {
        _typicalSpeakers = value;
        _updatedEvent.TypicalSpeakers = new Change<TypicalSpeakers>(value);
      }
    }
  }

  public Language() : base()
  {
  }

  public Language(WorldId worldId, Name name, UserId userId, Guid? entityId = null) : base(new LanguageId(worldId, entityId).AggregateId)
  {
    Raise(new CreatedEvent(name), userId.ActorId);
  }
  protected virtual void Apply(CreatedEvent @event)
  {
    _name = @event.Name;
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

    if (@event.Script != null)
    {
      _script = @event.Script.Value;
    }
    if (@event.TypicalSpeakers != null)
    {
      _typicalSpeakers = @event.TypicalSpeakers.Value;
    }
  }

  public override string ToString() => $"{Name.Value} | {base.ToString()}";

  public class CreatedEvent : DomainEvent, INotification
  {
    public Name Name { get; }

    public CreatedEvent(Name name)
    {
      Name = name;
    }
  }

  public class UpdatedEvent : DomainEvent, INotification
  {
    public Name? Name { get; set; }
    public Change<Description>? Description { get; set; }

    public Change<Script>? Script { get; set; }
    public Change<TypicalSpeakers>? TypicalSpeakers { get; set; }

    public bool HasChanges => Name != null || Description != null || Script != null || TypicalSpeakers != null;
  }
}
