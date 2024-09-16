using Logitar.EventSourcing;

namespace SkillCraft.Domain.Customizations;

public readonly struct CustomizationId
{
  public AggregateId AggregateId { get; }
  public string Value => AggregateId.Value;

  public CustomizationId(AggregateId actorId)
  {
    AggregateId = actorId;
  }
  public CustomizationId(Guid value)
  {
    AggregateId = new(value);
  }
  public CustomizationId(string value)
  {
    AggregateId = new(value);
  }

  public static CustomizationId NewId() => new(AggregateId.NewId());

  public Guid ToGuid() => AggregateId.ToGuid();

  public static bool operator ==(CustomizationId left, CustomizationId right) => left.Equals(right);
  public static bool operator !=(CustomizationId left, CustomizationId right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is CustomizationId userId && userId.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}
