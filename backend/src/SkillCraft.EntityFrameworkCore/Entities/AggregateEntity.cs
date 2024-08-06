using Logitar;
using Logitar.EventSourcing;

namespace SkillCraft.EntityFrameworkCore.Entities;

internal abstract class AggregateEntity
{
  public string AggregateId { get; private set; } = string.Empty;
  public long Version { get; private set; }

  public Guid CreatedBy { get; private set; }
  public DateTime CreatedOn { get; private set; }

  public Guid UpdatedBy { get; private set; }
  public DateTime UpdatedOn { get; private set; }

  protected AggregateEntity()
  {
  }

  protected AggregateEntity(DomainEvent @event)
  {
    AggregateId = @event.AggregateId.Value;

    CreatedBy = @event.ActorId.ToGuid();
    CreatedOn = @event.OccurredOn.AsUniversalTime();
  }

  public virtual IEnumerable<ActorId> GetActorIds() => [new(CreatedBy), new(UpdatedBy)];

  protected void Update(DomainEvent @event)
  {
    Version = @event.Version;

    UpdatedBy = @event.ActorId.ToGuid();
    UpdatedOn = @event.OccurredOn.AsUniversalTime();
  }
}
