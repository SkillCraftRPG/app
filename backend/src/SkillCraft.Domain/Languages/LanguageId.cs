using Logitar.EventSourcing;

namespace SkillCraft.Domain.Languages;

public readonly struct LanguageId
{
  public AggregateId AggregateId { get; }
  public string Value => AggregateId.Value;

  public LanguageId(AggregateId actorId)
  {
    AggregateId = actorId;
  }
  public LanguageId(Guid value)
  {
    AggregateId = new(value);
  }
  public LanguageId(string value)
  {
    AggregateId = new(value);
  }

  public static LanguageId NewId() => new(AggregateId.NewId());

  public Guid ToGuid() => AggregateId.ToGuid();

  public static bool operator ==(LanguageId left, LanguageId right) => left.Equals(right);
  public static bool operator !=(LanguageId left, LanguageId right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is LanguageId userId && userId.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}
