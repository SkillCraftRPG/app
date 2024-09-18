using Logitar.EventSourcing;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.EventSourcing.Infrastructure;
using SkillCraft.Domain.Languages;

namespace SkillCraft.EntityFrameworkCore.Repositories;

internal class LanguageRepository : Logitar.EventSourcing.EntityFrameworkCore.Relational.AggregateRepository, ILanguageRepository
{
  public LanguageRepository(IEventBus eventBus, EventContext eventContext, IEventSerializer eventSerializer)
    : base(eventBus, eventContext, eventSerializer)
  {
  }

  public async Task<IReadOnlyCollection<Language>> LoadAsync(CancellationToken cancellationToken)
  {
    return (await LoadAsync<Language>(cancellationToken)).ToArray();
  }

  public async Task<Language?> LoadAsync(LanguageId id, CancellationToken cancellationToken)
  {
    return await LoadAsync(id, version: null, cancellationToken);
  }
  public async Task<Language?> LoadAsync(LanguageId id, long? version, CancellationToken cancellationToken)
  {
    return await LoadAsync<Language>(id.AggregateId, version, cancellationToken);
  }

  public async Task<IReadOnlyCollection<Language>> LoadAsync(IEnumerable<LanguageId> ids, CancellationToken cancellationToken)
  {
    IEnumerable<AggregateId> aggregateIds = ids.Select(id => id.AggregateId).Distinct();
    return (await LoadAsync<Language>(aggregateIds, cancellationToken)).ToArray();
  }

  public async Task SaveAsync(Language language, CancellationToken cancellationToken)
  {
    await base.SaveAsync(language, cancellationToken);
  }
  public async Task SaveAsync(IEnumerable<Language> languages, CancellationToken cancellationToken)
  {
    await base.SaveAsync(languages, cancellationToken);
  }
}
