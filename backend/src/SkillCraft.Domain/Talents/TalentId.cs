using Logitar.EventSourcing;

namespace SkillCraft.Domain.Talents;

public readonly struct TalentId
{
  public AggregateId AggregateId { get; }
  public string Value => AggregateId.Value;

  public TalentId(AggregateId actorId)
  {
    AggregateId = actorId;
  }
  public TalentId(Guid value)
  {
    AggregateId = new(value);
  }
  public TalentId(string value)
  {
    AggregateId = new(value);
  }

  public static TalentId NewId() => new(AggregateId.NewId());

  public Guid ToGuid() => AggregateId.ToGuid();

  public static bool operator ==(TalentId left, TalentId right) => left.Equals(right);
  public static bool operator !=(TalentId left, TalentId right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is TalentId userId && userId.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}
