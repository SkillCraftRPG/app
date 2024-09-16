using Logitar.EventSourcing;

namespace SkillCraft.Domain.Speciez;

public readonly struct SpeciesId
{
  public AggregateId AggregateId { get; }
  public string Value => AggregateId.Value;

  public SpeciesId(AggregateId actorId)
  {
    AggregateId = actorId;
  }
  public SpeciesId(Guid value)
  {
    AggregateId = new(value);
  }
  public SpeciesId(string value)
  {
    AggregateId = new(value);
  }

  public static SpeciesId NewId() => new(AggregateId.NewId());

  public Guid ToGuid() => AggregateId.ToGuid();

  public static bool operator ==(SpeciesId left, SpeciesId right) => left.Equals(right);
  public static bool operator !=(SpeciesId left, SpeciesId right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is SpeciesId userId && userId.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}
