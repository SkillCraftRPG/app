using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.EventSourcing.Infrastructure;
using SkillCraft.Domain.Castes;

namespace SkillCraft.EntityFrameworkCore.Repositories;

internal class CasteRepository : Logitar.EventSourcing.EntityFrameworkCore.Relational.AggregateRepository, ICasteRepository
{
  public CasteRepository(IEventBus eventBus, EventContext eventContext, IEventSerializer eventSerializer)
    : base(eventBus, eventContext, eventSerializer)
  {
  }

  public async Task<IReadOnlyCollection<Caste>> LoadAsync(CancellationToken cancellationToken)
  {
    return (await LoadAsync<Caste>(cancellationToken)).ToArray();
  }

  public async Task<Caste?> LoadAsync(CasteId id, CancellationToken cancellationToken)
  {
    return await LoadAsync(id, version: null, cancellationToken);
  }
  public async Task<Caste?> LoadAsync(CasteId id, long? version, CancellationToken cancellationToken)
  {
    return await LoadAsync<Caste>(id.AggregateId, version, cancellationToken);
  }

  public async Task SaveAsync(Caste caste, CancellationToken cancellationToken)
  {
    await base.SaveAsync(caste, cancellationToken);
  }
  public async Task SaveAsync(IEnumerable<Caste> castes, CancellationToken cancellationToken)
  {
    await base.SaveAsync(castes, cancellationToken);
  }
}
