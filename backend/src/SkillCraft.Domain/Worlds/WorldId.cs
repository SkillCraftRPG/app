using Logitar.EventSourcing;
using System.Diagnostics.CodeAnalysis;

namespace SkillCraft.Domain.Worlds;

public readonly struct WorldId
{
  public AggregateId AggregateId { get; }
  public string Value => AggregateId.Value;

  public WorldId(AggregateId aggregateId)
  {
    AggregateId = aggregateId;
  }

  public static bool operator ==(WorldId left, WorldId right) => left.Equals(right);
  public static bool operator !=(WorldId left, WorldId right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is WorldId worldId && worldId.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}
