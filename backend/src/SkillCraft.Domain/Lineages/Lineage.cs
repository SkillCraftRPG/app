using Logitar.EventSourcing;
using MediatR;
using SkillCraft.Contracts;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Domain.Lineages;

public class Lineage : AggregateRoot
{
  private UpdatedEvent _updatedEvent = new();

  public new LineageId Id => new(base.Id);
  public WorldId WorldId => Id.WorldId;
  public Guid EntityId => Id.EntityId;

  public LineageId? ParentId { get; private set; }

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

  private AttributeBonuses _attributes = new();
  public AttributeBonuses Attributes
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
  private readonly Dictionary<Guid, Feature> _features = [];
  public IReadOnlyDictionary<Guid, Feature> Features => _features.AsReadOnly();

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
  private Weight _weight = new();
  public Weight Weight
  {
    get => _weight;
    set
    {
      if (_weight != value)
      {
        _weight = value;
        _updatedEvent.Weight = value;
      }
    }
  }
  private Ages _ages = new();
  public Ages Ages
  {
    get => _ages;
    set
    {
      if (_ages != value)
      {
        _ages = value;
        _updatedEvent.Ages = value;
      }
    }
  }

  public Lineage() : base()
  {
  }

  public Lineage(WorldId worldId, Lineage? parent, Name name, UserId userId, Guid? entityId = null) : base(new LineageId(worldId, entityId).AggregateId)
  {
    if (parent != null)
    {
      if (parent.WorldId != worldId)
      {
        throw new ArgumentException("The parent lineage does not reside in the same world as the lineage.", nameof(parent));
      }
      else if (parent.ParentId.HasValue)
      {
        throw new ArgumentException("The parent lineage cannot have a parent lineage.", nameof(parent));
      }
    }

    Raise(new CreatedEvent(parent?.Id, name), userId.ActorId);
  }
  protected virtual void Apply(CreatedEvent @event)
  {
    ParentId = @event.ParentId;

    _name = @event.Name;
  }

  public void AddFeature(Feature feature) => SetFeature(Guid.NewGuid(), feature);
  public void RemoveFeature(Guid id)
  {
    ArgumentOutOfRangeException.ThrowIfEqual(id, Guid.Empty, nameof(id));

    if (_features.Remove(id))
    {
      _updatedEvent.Features[id] = null;
    }
  }
  public void SetFeature(Guid id, Feature feature)
  {
    ArgumentOutOfRangeException.ThrowIfEqual(id, Guid.Empty, nameof(id));

    if (!_features.TryGetValue(id, out Feature? existingFeature) || existingFeature != feature)
    {
      _features[id] = feature;
      _updatedEvent.Features[id] = feature;
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
    foreach (KeyValuePair<Guid, Feature?> feature in @event.Features)
    {
      if (feature.Value == null)
      {
        _features.Remove(feature.Key);
      }
      else
      {
        _features[feature.Key] = feature.Value;
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
    if (@event.Weight != null)
    {
      _weight = @event.Weight;
    }
    if (@event.Ages != null)
    {
      _ages = @event.Ages;
    }
  }

  public override string ToString() => $"{Name.Value} | {base.ToString()}";

  public class CreatedEvent : DomainEvent, INotification
  {
    public LineageId? ParentId { get; }

    public Name Name { get; }

    public CreatedEvent(LineageId? parentId, Name name)
    {
      ParentId = parentId;

      Name = name;
    }
  }

  public class UpdatedEvent : DomainEvent, INotification
  {
    public Name? Name { get; set; }
    public Change<Description>? Description { get; set; }

    public AttributeBonuses? Attributes { get; set; }
    public Dictionary<Guid, Feature?> Features { get; set; } = [];

    public Languages? Languages { get; set; }
    public Names? Names { get; set; }

    public Speeds? Speeds { get; set; }
    public Size? Size { get; set; }
    public Weight? Weight { get; set; }
    public Ages? Ages { get; set; }

    public bool HasChanges => Name != null || Description != null
      || Attributes != null || Features.Count > 0 || Languages != null || Names != null
      || Speeds != null || Size != null || Weight != null || Ages != null;
  }
}
