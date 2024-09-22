using Logitar.EventSourcing;

namespace SkillCraft.Domain.Characters;

public readonly struct CharacterId
{
  public AggregateId AggregateId { get; }
  public string Value => AggregateId.Value;

  public CharacterId(AggregateId actorId)
  {
    AggregateId = actorId;
  }
  public CharacterId(Guid value)
  {
    AggregateId = new(value);
  }
  public CharacterId(string value)
  {
    AggregateId = new(value);
  }

  public static CharacterId NewId() => new(AggregateId.NewId());

  public Guid ToGuid() => AggregateId.ToGuid();

  public static bool operator ==(CharacterId left, CharacterId right) => left.Equals(right);
  public static bool operator !=(CharacterId left, CharacterId right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is CharacterId userId && userId.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}
