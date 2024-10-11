using Logitar.EventSourcing;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Domain.Lineages;

public readonly struct LineageId
{
  private const char Separator = ':';

  public WorldId WorldId { get; }
  public Guid EntityId { get; }
  public AggregateId AggregateId { get; }
  public string Value => AggregateId.Value;

  public LineageId(WorldId worldId, Guid? entityId = null)
  {
    WorldId = worldId;
    EntityId = entityId ?? Guid.NewGuid();
    AggregateId = new(string.Join(Separator, WorldId, new AggregateId(EntityId)));
  }
  public LineageId(AggregateId aggregateId) : this(aggregateId.Value)
  {
  }
  public LineageId(string value)
  {
    string[] values = value.Split(Separator);
    if (values.Length != 2)
    {
      throw new ArgumentException("The value is not a valid lineage ID.", nameof(value));
    }

    WorldId = new(values[0]);
    EntityId = new AggregateId(values[1]).ToGuid();
    AggregateId = new(value);
  }

  public static bool operator ==(LineageId left, LineageId right) => left.Equals(right);
  public static bool operator !=(LineageId left, LineageId right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is LineageId userId && userId.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}
