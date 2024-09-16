using Logitar.EventSourcing;

namespace SkillCraft.Domain.Lineages;

public readonly struct LineageId
{
  public AggregateId AggregateId { get; }
  public string Value => AggregateId.Value;

  public LineageId(AggregateId actorId)
  {
    AggregateId = actorId;
  }
  public LineageId(Guid value)
  {
    AggregateId = new(value);
  }
  public LineageId(string value)
  {
    AggregateId = new(value);
  }

  public static LineageId NewId() => new(AggregateId.NewId());

  public Guid ToGuid() => AggregateId.ToGuid();

  public static bool operator ==(LineageId left, LineageId right) => left.Equals(right);
  public static bool operator !=(LineageId left, LineageId right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is LineageId userId && userId.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}
