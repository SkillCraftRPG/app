using Logitar.EventSourcing;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Domain.Talents;

public readonly struct TalentId
{
  private const char Separator = ':';

  public WorldId WorldId { get; }
  public Guid EntityId { get; }
  public AggregateId AggregateId { get; }
  public string Value => AggregateId.Value;

  public TalentId(WorldId worldId, Guid? entityId = null)
  {
    WorldId = worldId;
    EntityId = entityId ?? Guid.NewGuid();
    AggregateId = new(string.Join(Separator, WorldId, new AggregateId(EntityId)));
  }
  public TalentId(AggregateId aggregateId) : this(aggregateId.Value)
  {
  }
  public TalentId(string value)
  {
    string[] values = value.Split(Separator);
    if (values.Length != 2)
    {
      throw new ArgumentException("The value is not a valid talent ID.", nameof(value));
    }

    WorldId = new(values[0]);
    EntityId = new AggregateId(values[1]).ToGuid();
    AggregateId = new(value);
  }

  public static bool operator ==(TalentId left, TalentId right) => left.Equals(right);
  public static bool operator !=(TalentId left, TalentId right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is TalentId userId && userId.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}
