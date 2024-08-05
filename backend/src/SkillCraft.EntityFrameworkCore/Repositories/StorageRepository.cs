using Logitar.EventSourcing;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.EventSourcing.Infrastructure;
using SkillCraft.Domain.Storage;

namespace SkillCraft.EntityFrameworkCore.Repositories;

internal class StorageRepository : Logitar.EventSourcing.EntityFrameworkCore.Relational.AggregateRepository, IStorageRepository
{
  public StorageRepository(IEventBus eventBus, EventContext eventContext, IEventSerializer eventSerializer)
    : base(eventBus, eventContext, eventSerializer)
  {
  }

  public async Task<StorageAggregate?> LoadAsync(Guid userId, CancellationToken cancellationToken)
  {
    AggregateId aggregateId = new(userId);
    return await LoadAsync<StorageAggregate>(aggregateId, cancellationToken);
  }
}
