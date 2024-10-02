using Logitar.EventSourcing;

namespace SkillCraft.Domain.Worlds;

public readonly struct WorldId
{
  public AggregateId AggregateId { get; }
  public string Value => AggregateId.Value;

  public WorldId(AggregateId actorId)
  {
    AggregateId = actorId;
  }
  public WorldId(Guid value)
  {
    AggregateId = new(value);
  }
  public WorldId(string value)
  {
    AggregateId = new(value);
  }

  public static WorldId NewId() => new(AggregateId.NewId());

  public Guid ToGuid() => AggregateId.ToGuid(); // TODO(fpion): elimininate all ToGuid

  public static bool operator ==(WorldId left, WorldId right) => left.Equals(right);
  public static bool operator !=(WorldId left, WorldId right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is WorldId userId && userId.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}
