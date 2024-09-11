using Logitar;
using Logitar.EventSourcing;

namespace SkillCraft.EntityFrameworkCore.Entities;

internal abstract class AggregateEntity
{
  public string AggregateId { get; private set; } = string.Empty;
  public long Version { get; private set; }

  public string CreatedBy { get; private set; } = string.Empty;
  public DateTime CreatedOn { get; private set; }

  public string UpdatedBy { get; private set; } = string.Empty;
  public DateTime UpdatedOn { get; private set; }

  protected AggregateEntity()
  {
  }

  protected AggregateEntity(DomainEvent @event)
  {
    AggregateId = @event.AggregateId.Value;

    CreatedBy = @event.ActorId.Value;
    CreatedOn = @event.OccurredOn.AsUniversalTime();

    Update(@event);
  }

  public virtual IEnumerable<ActorId> GetActorIds() => [new(CreatedBy), new(UpdatedBy)];

  public virtual void Update(DomainEvent @event)
  {
    Version = @event.Version;

    UpdatedBy = @event.ActorId.Value;
    UpdatedOn = @event.OccurredOn.AsUniversalTime();
  }

  public override bool Equals(object? obj) => obj is AggregateEntity aggregate && aggregate.GetType().Equals(GetType()) && aggregate.AggregateId == AggregateId;
  public override int GetHashCode() => HashCode.Combine(GetType(), AggregateId);
  public override string ToString() => $"{GetType()} (AggregateId={AggregateId})";
}
