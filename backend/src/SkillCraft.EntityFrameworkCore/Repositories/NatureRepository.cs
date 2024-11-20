using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.EventSourcing.Infrastructure;
using SkillCraft.Domain.Natures;

namespace SkillCraft.EntityFrameworkCore.Repositories;

internal class NatureRepository : Logitar.EventSourcing.EntityFrameworkCore.Relational.AggregateRepository, INatureRepository
{
  public NatureRepository(IEventBus eventBus, EventContext eventContext, IEventSerializer eventSerializer)
    : base(eventBus, eventContext, eventSerializer)
  {
  }

  public async Task<IReadOnlyCollection<Nature>> LoadAsync(CancellationToken cancellationToken)
  {
    return (await LoadAsync<Nature>(cancellationToken)).ToArray();
  }

  public async Task<Nature?> LoadAsync(NatureId id, CancellationToken cancellationToken)
  {
    return await LoadAsync(id, version: null, cancellationToken);
  }
  public async Task<Nature?> LoadAsync(NatureId id, long? version, CancellationToken cancellationToken)
  {
    return await LoadAsync<Nature>(id.AggregateId, version, cancellationToken);
  }

  public async Task SaveAsync(Nature nature, CancellationToken cancellationToken)
  {
    await base.SaveAsync(nature, cancellationToken);
  }
  public async Task SaveAsync(IEnumerable<Nature> natures, CancellationToken cancellationToken)
  {
    await base.SaveAsync(natures, cancellationToken);
  }
}
