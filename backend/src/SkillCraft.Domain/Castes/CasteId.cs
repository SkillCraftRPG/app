using Logitar.EventSourcing;

namespace SkillCraft.Domain.Castes;

public readonly struct CasteId
{
  public AggregateId AggregateId { get; }
  public string Value => AggregateId.Value;

  public CasteId(AggregateId actorId)
  {
    AggregateId = actorId;
  }
  public CasteId(Guid value)
  {
    AggregateId = new(value);
  }
  public CasteId(string value)
  {
    AggregateId = new(value);
  }

  public static CasteId NewId() => new(AggregateId.NewId());

  public Guid ToGuid() => AggregateId.ToGuid();

  public static bool operator ==(CasteId left, CasteId right) => left.Equals(right);
  public static bool operator !=(CasteId left, CasteId right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is CasteId userId && userId.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}
