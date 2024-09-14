using Logitar.EventSourcing;
using MediatR;
using SkillCraft.Contracts;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Domain.Castes;

public class Caste : AggregateRoot
{
  private UpdatedEvent _updatedEvent = new();

  public new CasteId Id => new(base.Id);

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

  private Skill? _skill = null;
  public Skill? Skill
  {
    get => _skill;
    set
    {
      if (_skill != value)
      {
        if (value.HasValue && !Enum.IsDefined(value.Value))
        {
          throw new ArgumentOutOfRangeException(nameof(Skill));
        }

        _skill = value;
        _updatedEvent.Skill = new Change<Skill?>(value);
      }
    }
  }
  private Roll? _wealthRoll = null;
  public Roll? WealthRoll
  {
    get => _wealthRoll;
    set
    {
      if (_wealthRoll != value)
      {
        _wealthRoll = value;
        _updatedEvent.WealthRoll = new Change<Roll>(value);
      }
    }
  }

  private readonly Dictionary<Guid, Trait> _traits = [];
  public IReadOnlyDictionary<Guid, Trait> Traits => _traits.AsReadOnly();

  public Caste() : base()
  {
  }

  public Caste(WorldId worldId, Name name, UserId userId, CasteId? id = null) : base(id?.AggregateId)
  {
    Raise(new CreatedEvent(worldId, name), userId.ActorId);
  }
  protected virtual void Apply(CreatedEvent @event)
  {
    WorldId = @event.WorldId;

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

    if (@event.Skill != null)
    {
      _skill = @event.Skill.Value;
    }
    if (@event.WealthRoll != null)
    {
      _wealthRoll = @event.WealthRoll.Value;
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
  }

  public override string ToString() => $"{Name.Value} | {base.ToString()}";

  public class CreatedEvent : DomainEvent, INotification
  {
    public WorldId WorldId { get; }

    public Name Name { get; }

    public CreatedEvent(WorldId worldId, Name name)
    {
      WorldId = worldId;

      Name = name;
    }
  }

  public class UpdatedEvent : DomainEvent, INotification
  {
    public Name? Name { get; set; }
    public Change<Description>? Description { get; set; }

    public Change<Skill?>? Skill { get; set; }
    public Change<Roll>? WealthRoll { get; set; }

    public Dictionary<Guid, Trait?> Traits { get; set; } = [];

    public bool HasChanges => Name != null || Description != null || Skill != null || WealthRoll != null || Traits.Count > 0;
  }
}
