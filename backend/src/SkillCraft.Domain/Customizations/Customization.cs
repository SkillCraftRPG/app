using Logitar.EventSourcing;
using MediatR;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Customizations;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Domain.Customizations;

public class Customization : AggregateRoot
{
  private UpdatedEvent _updatedEvent = new();

  public new CustomizationId Id => new(base.Id);

  public WorldId WorldId { get; private set; }

  public CustomizationType Type { get; private set; }

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

  public Customization() : base()
  {
  }

  public Customization(WorldId worldId, CustomizationType type, Name name, UserId userId, CustomizationId? id = null) : base(id?.AggregateId)
  {
    if (!Enum.IsDefined(type))
    {
      throw new ArgumentOutOfRangeException(nameof(type));
    }

    Raise(new CreatedEvent(worldId, type, name), userId.ActorId);
  }
  protected virtual void Apply(CreatedEvent @event)
  {
    WorldId = @event.WorldId;

    Type = @event.Type;

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
  }

  public override string ToString() => $"{Name.Value} | {base.ToString()}";

  public class CreatedEvent : DomainEvent, INotification
  {
    public WorldId WorldId { get; }

    public CustomizationType Type { get; }

    public Name Name { get; }

    public CreatedEvent(WorldId worldId, CustomizationType type, Name name)
    {
      WorldId = worldId;

      Type = type;

      Name = name;
    }
  }

  public class UpdatedEvent : DomainEvent, INotification
  {
    public Name? Name { get; set; }
    public Change<Description>? Description { get; set; }

    public bool HasChanges => Name != null || Description != null;
  }
}
