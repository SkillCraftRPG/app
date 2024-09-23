using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.EventSourcing.Infrastructure;
using SkillCraft.Domain.Talents;

namespace SkillCraft.EntityFrameworkCore.Repositories;

internal class TalentRepository : Logitar.EventSourcing.EntityFrameworkCore.Relational.AggregateRepository, ITalentRepository
{
  public TalentRepository(IEventBus eventBus, EventContext eventContext, IEventSerializer eventSerializer)
    : base(eventBus, eventContext, eventSerializer)
  {
  }

  public async Task<IReadOnlyCollection<Talent>> LoadAsync(CancellationToken cancellationToken)
  {
    return (await LoadAsync<Talent>(cancellationToken)).ToArray();
  }

  public async Task<Talent?> LoadAsync(TalentId id, CancellationToken cancellationToken)
  {
    return await LoadAsync(id, version: null, cancellationToken);
  }
  public async Task<Talent?> LoadAsync(TalentId id, long? version, CancellationToken cancellationToken)
  {
    return await LoadAsync<Talent>(id.AggregateId, version, cancellationToken);
  }

  public async Task SaveAsync(Talent talent, CancellationToken cancellationToken)
  {
    await base.SaveAsync(talent, cancellationToken);
  }
  public async Task SaveAsync(IEnumerable<Talent> talents, CancellationToken cancellationToken)
  {
    await base.SaveAsync(talents, cancellationToken);
  }
}
