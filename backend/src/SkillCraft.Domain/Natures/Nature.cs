using Logitar.EventSourcing;
using MediatR;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Customizations;
using SkillCraft.Domain.Customizations;
using SkillCraft.Domain.Worlds;
using Attribute = SkillCraft.Contracts.Attribute;

namespace SkillCraft.Domain.Natures;

public class Nature : AggregateRoot
{
  private UpdatedEvent _updatedEvent = new();

  public new NatureId Id => new(base.Id);
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

  private Attribute? _attribute = null;
  public Attribute? Attribute
  {
    get => _attribute;
    set
    {
      if (_attribute != value)
      {
        if (value.HasValue && !Enum.IsDefined(value.Value))
        {
          throw new ArgumentOutOfRangeException(nameof(Attribute));
        }

        _attribute = value;
        _updatedEvent.Attribute = new Change<Attribute?>(value);
      }
    }
  }
  public CustomizationId? GiftId { get; private set; }

  public Nature() : base()
  {
  }

  public Nature(WorldId worldId, Name name, UserId userId, Guid? entityId = null) : base(new NatureId(worldId, entityId).AggregateId)
  {
    Raise(new CreatedEvent(name), userId.ActorId);
  }
  protected virtual void Apply(CreatedEvent @event)
  {
    _name = @event.Name;
  }

  public void SetGift(Customization? gift)
  {
    if (gift != null)
    {
      if (gift.WorldId != WorldId)
      {
        throw new ArgumentException("The gift does not reside in the same world as the nature.", nameof(gift));
      }
      else if (gift.Type != CustomizationType.Gift)
      {
        throw new CustomizationIsNotGiftException(gift, nameof(GiftId));
      }
    }

    if (GiftId != gift?.Id)
    {
      GiftId = gift?.Id;
      _updatedEvent.GiftId = new Change<CustomizationId?>(gift?.Id);
    }
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

    if (@event.Attribute != null)
    {
      _attribute = @event.Attribute.Value;
    }
    if (@event.GiftId != null)
    {
      GiftId = @event.GiftId.Value;
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

    public Change<Attribute?>? Attribute { get; set; }
    public Change<CustomizationId?>? GiftId { get; set; }

    public bool HasChanges => Name != null || Description != null || Attribute != null || GiftId != null;
  }
}
