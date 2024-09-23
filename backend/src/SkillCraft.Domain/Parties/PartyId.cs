using Logitar.EventSourcing;

namespace SkillCraft.Domain.Parties;

public readonly struct PartyId
{
  public AggregateId AggregateId { get; }
  public string Value => AggregateId.Value;

  public PartyId(AggregateId actorId)
  {
    AggregateId = actorId;
  }
  public PartyId(Guid value)
  {
    AggregateId = new(value);
  }
  public PartyId(string value)
  {
    AggregateId = new(value);
  }

  public static PartyId NewId() => new(AggregateId.NewId());

  public Guid ToGuid() => AggregateId.ToGuid();

  public static bool operator ==(PartyId left, PartyId right) => left.Equals(right);
  public static bool operator !=(PartyId left, PartyId right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is PartyId userId && userId.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}
