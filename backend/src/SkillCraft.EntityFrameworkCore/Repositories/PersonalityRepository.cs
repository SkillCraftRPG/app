using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.EventSourcing.Infrastructure;
using SkillCraft.Domain.Personalitys;

namespace SkillCraft.EntityFrameworkCore.Repositories;

internal class PersonalityRepository : Logitar.EventSourcing.EntityFrameworkCore.Relational.AggregateRepository, IPersonalityRepository
{
  public PersonalityRepository(IEventBus eventBus, EventContext eventContext, IEventSerializer eventSerializer)
    : base(eventBus, eventContext, eventSerializer)
  {
  }

  public async Task<IReadOnlyCollection<Personality>> LoadAsync(CancellationToken cancellationToken)
  {
    return (await LoadAsync<Personality>(cancellationToken)).ToArray();
  }

  public async Task<Personality?> LoadAsync(PersonalityId id, CancellationToken cancellationToken)
  {
    return await LoadAsync(id, version: null, cancellationToken);
  }
  public async Task<Personality?> LoadAsync(PersonalityId id, long? version, CancellationToken cancellationToken)
  {
    return await LoadAsync<Personality>(id.AggregateId, version, cancellationToken);
  }

  public async Task SaveAsync(Personality personality, CancellationToken cancellationToken)
  {
    await base.SaveAsync(personality, cancellationToken);
  }
  public async Task SaveAsync(IEnumerable<Personality> personalitys, CancellationToken cancellationToken)
  {
    await base.SaveAsync(personalitys, cancellationToken);
  }
}
