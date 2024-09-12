using Logitar.EventSourcing;
using MediatR;
using SkillCraft.Contracts;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Domain.Educations;

public class Education : AggregateRoot
{
  private UpdatedEvent _updatedEvent = new();

  public new EducationId Id => new(base.Id);

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
  private double? _wealthMultiplier = null;
  public double? WealthMultiplier
  {
    get => _wealthMultiplier;
    set
    {
      if (_wealthMultiplier != value)
      {
        if (value <= 0.0)
        {
          throw new ArgumentOutOfRangeException(nameof(WealthMultiplier));
        }

        _wealthMultiplier = value;
        _updatedEvent.WealthMultiplier = new Change<double?>(value);
      }
    }
  }

  public Education() : base()
  {
  }

  public Education(WorldId worldId, Name name, UserId userId, EducationId? id = null) : base(id?.AggregateId)
  {
    Raise(new CreatedEvent(worldId, name), userId.ActorId);
  }
  protected virtual void Apply(CreatedEvent @event)
  {
    WorldId = @event.WorldId;

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

    if (@event.Skill != null)
    {
      _skill = @event.Skill.Value;
    }
    if (@event.WealthMultiplier != null)
    {
      _wealthMultiplier = @event.WealthMultiplier.Value;
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
    public Change<double?>? WealthMultiplier { get; set; }

    public bool HasChanges => Name != null || Description != null || Skill != null || WealthMultiplier != null;
  }
}
