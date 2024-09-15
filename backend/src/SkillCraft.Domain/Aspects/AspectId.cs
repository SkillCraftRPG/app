using Logitar.EventSourcing;

namespace SkillCraft.Domain.Aspects;

public readonly struct AspectId
{
  public AggregateId AggregateId { get; }
  public string Value => AggregateId.Value;

  public AspectId(AggregateId actorId)
  {
    AggregateId = actorId;
  }
  public AspectId(Guid value)
  {
    AggregateId = new(value);
  }
  public AspectId(string value)
  {
    AggregateId = new(value);
  }

  public static AspectId NewId() => new(AggregateId.NewId());

  public Guid ToGuid() => AggregateId.ToGuid();

  public static bool operator ==(AspectId left, AspectId right) => left.Equals(right);
  public static bool operator !=(AspectId left, AspectId right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is AspectId userId && userId.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}
