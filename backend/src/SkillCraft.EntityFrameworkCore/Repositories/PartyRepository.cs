using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.EventSourcing.Infrastructure;
using SkillCraft.Domain.Parties;

namespace SkillCraft.EntityFrameworkCore.Repositories;

internal class PartyRepository : Logitar.EventSourcing.EntityFrameworkCore.Relational.AggregateRepository, IPartyRepository
{
  public PartyRepository(IEventBus eventBus, EventContext eventContext, IEventSerializer eventSerializer)
    : base(eventBus, eventContext, eventSerializer)
  {
  }

  public async Task<IReadOnlyCollection<Party>> LoadAsync(CancellationToken cancellationToken)
  {
    return (await LoadAsync<Party>(cancellationToken)).ToArray();
  }

  public async Task<Party?> LoadAsync(PartyId id, CancellationToken cancellationToken)
  {
    return await LoadAsync(id, version: null, cancellationToken);
  }
  public async Task<Party?> LoadAsync(PartyId id, long? version, CancellationToken cancellationToken)
  {
    return await LoadAsync<Party>(id.AggregateId, version, cancellationToken);
  }

  public async Task SaveAsync(Party party, CancellationToken cancellationToken)
  {
    await base.SaveAsync(party, cancellationToken);
  }
  public async Task SaveAsync(IEnumerable<Party> parties, CancellationToken cancellationToken)
  {
    await base.SaveAsync(parties, cancellationToken);
  }
}
