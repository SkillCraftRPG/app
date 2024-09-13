using Logitar.EventSourcing;

namespace SkillCraft.Domain.Educations;

public readonly struct EducationId
{
  public AggregateId AggregateId { get; }
  public string Value => AggregateId.Value;

  public EducationId(AggregateId actorId)
  {
    AggregateId = actorId;
  }
  public EducationId(Guid value)
  {
    AggregateId = new(value);
  }
  public EducationId(string value)
  {
    AggregateId = new(value);
  }

  public static EducationId NewId() => new(AggregateId.NewId());

  public Guid ToGuid() => AggregateId.ToGuid();

  public static bool operator ==(EducationId left, EducationId right) => left.Equals(right);
  public static bool operator !=(EducationId left, EducationId right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is EducationId userId && userId.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}
