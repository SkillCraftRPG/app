using Logitar.EventSourcing;
using Logitar.Identity.Contracts;
using Logitar.Identity.Domain.Shared;
using SkillCraft.Domain.Worlds.Events;

namespace SkillCraft.Domain.Worlds;

public class WorldAggregate : AggregateRoot
{
  private WorldUpdatedEvent _updatedEvent = new();

  public new WorldId Id => new(base.Id);

  public ActorId OwnerId { get; private set; }

  private SlugUnit? _uniqueSlug = null;
  public SlugUnit UniqueSlug
  {
    get => _uniqueSlug ?? throw new InvalidOperationException($"The {nameof(UniqueSlug)} has not been initialized yet.");
    set
    {
      if (value != _uniqueSlug)
      {
        _uniqueSlug = value;
        _updatedEvent.UniqueSlug = value;
      }
    }
  }
  private DisplayNameUnit? _displayName = null;
  public DisplayNameUnit? DisplayName
  {
    get => _displayName;
    set
    {
      if (value != _displayName)
      {
        _displayName = value;
        _updatedEvent.DisplayName = new Modification<DisplayNameUnit>(value);
      }
    }
  }
  private DescriptionUnit? _description = null;
  public DescriptionUnit? Description
  {
    get => _description;
    set
    {
      if (value != _description)
      {
        _description = value;
        _updatedEvent.Description = new Modification<DescriptionUnit>(value);
      }
    }
  }

  public WorldAggregate() : base()
  {
  }

  public WorldAggregate(SlugUnit uniqueSlug, ActorId actorId = default, WorldId? id = null) : base(id?.AggregateId)
  {
    Raise(new WorldCreatedEvent(uniqueSlug), actorId);
  }
  protected virtual void Apply(WorldCreatedEvent @event)
  {
    OwnerId = @event.ActorId;

    _uniqueSlug = @event.UniqueSlug;
  }

  public void Update(ActorId actorId = default)
  {
    if (_updatedEvent.HasChanges)
    {
      Raise(_updatedEvent, actorId, DateTime.Now);
      _updatedEvent = new();
    }
  }
  protected virtual void Apply(WorldUpdatedEvent @event)
  {
    if (@event.UniqueSlug != null)
    {
      _uniqueSlug = @event.UniqueSlug;
    }
    if (@event.DisplayName != null)
    {
      _displayName = @event.DisplayName.Value;
    }
    if (@event.Description != null)
    {
      _description = @event.Description.Value;
    }
  }

  public override string ToString() => $"{DisplayName?.Value ?? UniqueSlug.Value} | {base.ToString()}";
}
