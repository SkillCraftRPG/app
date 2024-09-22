using Logitar.EventSourcing;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.EventSourcing.Infrastructure;
using SkillCraft.Domain.Customizations;

namespace SkillCraft.EntityFrameworkCore.Repositories;

internal class CustomizationRepository : Logitar.EventSourcing.EntityFrameworkCore.Relational.AggregateRepository, ICustomizationRepository
{
  public CustomizationRepository(IEventBus eventBus, EventContext eventContext, IEventSerializer eventSerializer)
    : base(eventBus, eventContext, eventSerializer)
  {
  }

  public async Task<IReadOnlyCollection<Customization>> LoadAsync(CancellationToken cancellationToken)
  {
    return (await LoadAsync<Customization>(cancellationToken)).ToArray();
  }

  public async Task<Customization?> LoadAsync(CustomizationId id, CancellationToken cancellationToken)
  {
    return await LoadAsync(id, version: null, cancellationToken);
  }
  public async Task<Customization?> LoadAsync(CustomizationId id, long? version, CancellationToken cancellationToken)
  {
    return await LoadAsync<Customization>(id.AggregateId, version, cancellationToken);
  }

  public async Task<IReadOnlyCollection<Customization>> LoadAsync(IEnumerable<CustomizationId> ids, CancellationToken cancellationToken)
  {
    IEnumerable<AggregateId> aggregateIds = ids.Distinct().Select(id => id.AggregateId);
    return (await LoadAsync<Customization>(aggregateIds, cancellationToken)).ToArray();
  }

  public async Task SaveAsync(Customization customization, CancellationToken cancellationToken)
  {
    await base.SaveAsync(customization, cancellationToken);
  }
  public async Task SaveAsync(IEnumerable<Customization> customizations, CancellationToken cancellationToken)
  {
    await base.SaveAsync(customizations, cancellationToken);
  }
}
