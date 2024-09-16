using Logitar.EventSourcing;
using MediatR;
using SkillCraft.Contracts;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Domain.Speciez;

public class Species : AggregateRoot
{
  private UpdatedEvent _updatedEvent = new();

  public new SpeciesId Id => new(base.Id);

  public WorldId WorldId { get; private set; }

  public SpeciesId? ParentId { get; private set; }

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
  private readonly Dictionary<Guid, Trait> _traits = [];
  public IReadOnlyDictionary<Guid, Trait> Traits => _traits.AsReadOnly();

  private Languages? _languages = null;
  public Languages Languages
  {
    get => _languages ?? throw new InvalidOperationException($"The {nameof(Languages)} has not been initialized yet.");
    set
    {
      if (_languages != value)
      {
        _languages = value;
        _updatedEvent.Languages = value;
      }
    }
  }

  // TODO(fpion): Names (…)
  // TODO(fpion): Speed
  // TODO(fpion): Size (Category, Roll)
  // TODO(fpion): Weight (5x Roll)
  // TODO(fpion): Age (4x Lower Inclusive Bound, Vodyanoi ex.: 8, 15, 40, 70)

  public Species() : base()
  {
  }

  public Species(WorldId worldId, SpeciesId? parentId, Name name, UserId userId, SpeciesId? id = null) : base(id?.AggregateId)
  {
    Raise(new CreatedEvent(worldId, parentId, name, new Attributes(), new Languages()), userId.ActorId);
  }
  protected virtual void Apply(CreatedEvent @event)
  {
    WorldId = @event.WorldId;

    ParentId = @event.ParentId;

    _name = @event.Name;

    _attributes = @event.Attributes;
    _languages = @event.Languages;
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
  }

  public override string ToString() => $"{Name.Value} | {base.ToString()}";

  public class CreatedEvent : DomainEvent, INotification
  {
    public WorldId WorldId { get; }

    public SpeciesId? ParentId { get; }

    public Name Name { get; }

    public Attributes Attributes { get; }
    public Languages Languages { get; }

    public CreatedEvent(WorldId worldId, SpeciesId? parentId, Name name, Attributes attributes, Languages languages)
    {
      WorldId = worldId;

      ParentId = parentId;

      Name = name;

      Attributes = attributes;
      Languages = languages;
    }
  }

  public class UpdatedEvent : DomainEvent, INotification
  {
    public Name? Name { get; set; }
    public Change<Description>? Description { get; set; }

    public Attributes? Attributes { get; set; }
    public Dictionary<Guid, Trait?> Traits { get; set; } = [];

    public Languages? Languages { get; set; }

    public bool HasChanges => Name != null || Description != null || Attributes != null || Traits.Count > 0 || Languages != null;
  }
}
