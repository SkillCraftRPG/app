using Logitar.EventSourcing;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Domain.Castes;

public readonly struct CasteId
{
  private const char Separator = ':';

  public WorldId WorldId { get; }
  public Guid EntityId { get; }
  public AggregateId AggregateId { get; }
  public string Value => AggregateId.Value;

  public CasteId(WorldId worldId, Guid? entityId = null)
  {
    WorldId = worldId;
    EntityId = entityId ?? Guid.NewGuid();
    AggregateId = new(string.Join(Separator, WorldId, new AggregateId(EntityId)));
  }
  public CasteId(AggregateId aggregateId) : this(aggregateId.Value)
  {
  }
  public CasteId(string value)
  {
    string[] values = value.Split(Separator);
    if (values.Length != 2)
    {
      throw new ArgumentException("The value is not a valid caste ID.", nameof(value));
    }

    WorldId = new(values[0]);
    EntityId = new AggregateId(values[1]).ToGuid();
    AggregateId = new(value);
  }

  public static bool operator ==(CasteId left, CasteId right) => left.Equals(right);
  public static bool operator !=(CasteId left, CasteId right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is CasteId userId && userId.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}
