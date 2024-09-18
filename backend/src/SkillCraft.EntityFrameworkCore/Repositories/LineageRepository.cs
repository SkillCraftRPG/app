using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.EventSourcing.Infrastructure;
using SkillCraft.Domain.Lineages;

namespace SkillCraft.EntityFrameworkCore.Repositories;

internal class LineageRepository : Logitar.EventSourcing.EntityFrameworkCore.Relational.AggregateRepository, ILineageRepository
{
  public LineageRepository(IEventBus eventBus, EventContext eventContext, IEventSerializer eventSerializer)
    : base(eventBus, eventContext, eventSerializer)
  {
  }

  public async Task<IReadOnlyCollection<Lineage>> LoadAsync(CancellationToken cancellationToken)
  {
    return (await LoadAsync<Lineage>(cancellationToken)).ToArray();
  }

  public async Task<Lineage?> LoadAsync(LineageId id, CancellationToken cancellationToken)
  {
    return await LoadAsync(id, version: null, cancellationToken);
  }
  public async Task<Lineage?> LoadAsync(LineageId id, long? version, CancellationToken cancellationToken)
  {
    return await LoadAsync<Lineage>(id.AggregateId, version, cancellationToken);
  }

  public async Task SaveAsync(Lineage lineage, CancellationToken cancellationToken)
  {
    await base.SaveAsync(lineage, cancellationToken);
  }
  public async Task SaveAsync(IEnumerable<Lineage> lineages, CancellationToken cancellationToken)
  {
    await base.SaveAsync(lineages, cancellationToken);
  }
}
