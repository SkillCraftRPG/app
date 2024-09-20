using Logitar.EventSourcing;
using MediatR;
using SkillCraft.Domain.Lineages;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Domain.Characters;

public class Character : AggregateRoot
{
  public new CharacterId Id => new(base.Id);

  public WorldId WorldId { get; private set; }

  private Name? _name = null;
  public Name Name => _name ?? throw new InvalidOperationException($"The {nameof(Name)} has not been initialized yet.");
  public PlayerName? Player { get; private set; }

  public LineageId LineageId { get; private set; }
  public double Height { get; private set; }
  public double Weight { get; private set; }
  public int Age { get; private set; }

  public Character() : base()
  {
  }

  public Character(WorldId worldId, Name name, PlayerName? player, Lineage lineage, double height, double weight, int age, UserId userId, CharacterId? id = null)
    : base(id?.AggregateId)
  {
    if (lineage.WorldId != worldId)
    {
      throw new ArgumentException("The lineage does not reside in the same world as the character.", nameof(lineage));
    }
    ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(height, 0.0, nameof(height));
    ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(weight, 0.0, nameof(weight));
    ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(age, 0, nameof(age));

    Raise(new CreatedEvent(worldId, name, player, lineage.Id, height, weight, age), userId.ActorId);
  }
  protected virtual void Apply(CreatedEvent @event)
  {
    WorldId = @event.WorldId;

    _name = @event.Name;
    Player = @event.Player;

    LineageId = @event.LineageId;
    Height = @event.Height;
    Weight = @event.Weight;
    Age = @event.Age;
  }

  public override string ToString() => $"{Name} | {base.ToString()}";

  public class CreatedEvent : DomainEvent, INotification
  {
    public WorldId WorldId { get; }

    public Name Name { get; }
    public PlayerName? Player { get; }

    public LineageId LineageId { get; }
    public double Height { get; }
    public double Weight { get; }
    public int Age { get; }

    public CreatedEvent(WorldId worldId, Name name, PlayerName? player, LineageId lineageId, double height, double weight, int age)
    {
      WorldId = worldId;

      Name = name;
      Player = player;

      LineageId = lineageId;
      Height = height;
      Weight = weight;
      Age = age;
    }
  }
}
