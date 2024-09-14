using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.EventSourcing.Infrastructure;
using SkillCraft.Domain.Aspects;

namespace SkillCraft.EntityFrameworkCore.Repositories;

internal class AspectRepository : Logitar.EventSourcing.EntityFrameworkCore.Relational.AggregateRepository, IAspectRepository
{
  public AspectRepository(IEventBus eventBus, EventContext eventContext, IEventSerializer eventSerializer)
    : base(eventBus, eventContext, eventSerializer)
  {
  }

  public async Task<IReadOnlyCollection<Aspect>> LoadAsync(CancellationToken cancellationToken)
  {
    return (await LoadAsync<Aspect>(cancellationToken)).ToArray();
  }

  public async Task<Aspect?> LoadAsync(AspectId id, CancellationToken cancellationToken)
  {
    return await LoadAsync(id, version: null, cancellationToken);
  }
  public async Task<Aspect?> LoadAsync(AspectId id, long? version, CancellationToken cancellationToken)
  {
    return await LoadAsync<Aspect>(id.AggregateId, version, cancellationToken);
  }

  public async Task SaveAsync(Aspect aspect, CancellationToken cancellationToken)
  {
    await base.SaveAsync(aspect, cancellationToken);
  }
  public async Task SaveAsync(IEnumerable<Aspect> aspects, CancellationToken cancellationToken)
  {
    await base.SaveAsync(aspects, cancellationToken);
  }
}
