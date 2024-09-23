using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.EventSourcing.Infrastructure;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.EntityFrameworkCore.Repositories;

internal class WorldRepository : Logitar.EventSourcing.EntityFrameworkCore.Relational.AggregateRepository, IWorldRepository
{
  public WorldRepository(IEventBus eventBus, EventContext eventContext, IEventSerializer eventSerializer)
    : base(eventBus, eventContext, eventSerializer)
  {
  }

  public async Task<IReadOnlyCollection<World>> LoadAsync(CancellationToken cancellationToken)
  {
    return (await LoadAsync<World>(cancellationToken)).ToArray();
  }

  public async Task<World?> LoadAsync(WorldId id, CancellationToken cancellationToken)
  {
    return await LoadAsync(id, version: null, cancellationToken);
  }
  public async Task<World?> LoadAsync(WorldId id, long? version, CancellationToken cancellationToken)
  {
    return await LoadAsync<World>(id.AggregateId, version, cancellationToken);
  }

  public async Task SaveAsync(World world, CancellationToken cancellationToken)
  {
    await base.SaveAsync(world, cancellationToken);
  }
  public async Task SaveAsync(IEnumerable<World> worlds, CancellationToken cancellationToken)
  {
    await base.SaveAsync(worlds, cancellationToken);
  }
}
