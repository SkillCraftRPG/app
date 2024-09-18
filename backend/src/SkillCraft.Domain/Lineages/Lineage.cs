using Logitar.EventSourcing;
using MediatR;
using SkillCraft.Contracts;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Domain.Lineages;

public class Lineage : AggregateRoot
{
  private UpdatedEvent _updatedEvent = new();

  public new LineageId Id => new(base.Id);

  public LineageId? ParentId { get; private set; }

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

  private Attributes _attributes = new();
  public Attributes Attributes
  {
    get => _attributes;
    set
    {
      if (_attributes != value)
      {
        _attributes = value;
        _updatedEvent.Attributes = value;
      }
    }
  }
  private readonly Dictionary<Guid, Trait> _traits = [];
  public IReadOnlyDictionary<Guid, Trait> Traits => _traits.AsReadOnly();

  private Languages _languages = new();
  public Languages Languages
  {
    get => _languages;
    set
    {
      if (_languages != value)
      {
        _languages = value;
        _updatedEvent.Languages = value;
      }
    }
  }
  private Names _names = new();
  public Names Names
  {
    get => _names;
    set
    {
      if (_names != value)
      {
        _names = value;
        _updatedEvent.Names = value;
      }
    }
  }

  private Speeds _speeds = new();
  public Speeds Speeds
  {
    get => _speeds;
    set
    {
      if (_speeds != value)
      {
        _speeds = value;
        _updatedEvent.Speeds = value;
      }
    }
  }
  private Size _size = new();
  public Size Size
  {
    get => _size;
    set
    {
      if (_size != value)
      {
        _size = value;
        _updatedEvent.Size = value;
      }
    }
  }

  public Lineage() : base()
  {
  }

  public Lineage(WorldId worldId, Lineage? parent, Name name, UserId userId, LineageId? id = null) : base(id?.AggregateId)
  {
    if (parent?.ParentId != null)
    {
      throw new NotImplementedException(); // TODO(fpion): typed exception
    }

    Raise(new CreatedEvent(worldId, parent?.Id, name), userId.ActorId);
  }
  protected virtual void Apply(CreatedEvent @event)
  {
    WorldId = @event.WorldId;

    ParentId = @event.ParentId;

    _name = @event.Name;
  }

  public void AddTrait(Trait trait) => SetTrait(Guid.NewGuid(), trait);
  public void RemoveTrait(Guid id)
  {
    ArgumentOutOfRangeException.ThrowIfEqual(id, Guid.Empty, nameof(id));

    if (_traits.Remove(id))
    {
      _updatedEvent.Traits[id] = null;
    }
  }
  public void SetTrait(Guid id, Trait trait)
  {
    ArgumentOutOfRangeException.ThrowIfEqual(id, Guid.Empty, nameof(id));

    if (!_traits.TryGetValue(id, out Trait? existingTrait) || existingTrait != trait)
    {
      _traits[id] = trait;
      _updatedEvent.Traits[id] = trait;
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

    if (@event.Attributes != null)
    {
      _attributes = @event.Attributes;
    }
    foreach (KeyValuePair<Guid, Trait?> trait in @event.Traits)
    {
      if (trait.Value == null)
      {
        _traits.Remove(trait.Key);
      }
      else
      {
        _traits[trait.Key] = trait.Value;
      }
    }

    if (@event.Languages != null)
    {
      _languages = @event.Languages;
    }
    if (@event.Names != null)
    {
      _names = @event.Names;
    }

    if (@event.Speeds != null)
    {
      _speeds = @event.Speeds;
    }
    if (@event.Size != null)
    {
      _size = @event.Size;
    }
  }

  public override string ToString() => $"{Name.Value} | {base.ToString()}";

  public class CreatedEvent : DomainEvent, INotification
  {
    public WorldId WorldId { get; }

    public LineageId? ParentId { get; }

    public Name Name { get; }

    public CreatedEvent(WorldId worldId, LineageId? parentId, Name name)
    {
      WorldId = worldId;

      ParentId = parentId;

      Name = name;
    }
  }

  public class UpdatedEvent : DomainEvent, INotification
  {
    public Name? Name { get; set; }
    public Change<Description>? Description { get; set; }

    public Attributes? Attributes { get; set; }
    public Dictionary<Guid, Trait?> Traits { get; set; } = [];

    public Languages? Languages { get; set; }
    public Names? Names { get; set; }

    public Speeds? Speeds { get; set; }
    public Size? Size { get; set; }

    public bool HasChanges => Name != null || Description != null
      || Attributes != null || Traits.Count > 0 || Languages != null || Names != null
      || Speeds != null || Size != null;
  }
}
