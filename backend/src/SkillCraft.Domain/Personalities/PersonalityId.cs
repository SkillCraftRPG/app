using Logitar.EventSourcing;

namespace SkillCraft.Domain.Personalities;

public readonly struct PersonalityId
{
  public AggregateId AggregateId { get; }
  public string Value => AggregateId.Value;

  public PersonalityId(AggregateId actorId)
  {
    AggregateId = actorId;
  }
  public PersonalityId(Guid value)
  {
    AggregateId = new(value);
  }
  public PersonalityId(string value)
  {
    AggregateId = new(value);
  }

  public static PersonalityId NewId() => new(AggregateId.NewId());

  public Guid ToGuid() => AggregateId.ToGuid();

  public static bool operator ==(PersonalityId left, PersonalityId right) => left.Equals(right);
  public static bool operator !=(PersonalityId left, PersonalityId right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is PersonalityId userId && userId.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}
