using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.EventSourcing.Infrastructure;
using SkillCraft.Application.Worlds;
using SkillCraft.Domain;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.EntityFrameworkCore.Repositories;

internal class WorldRepository : Logitar.EventSourcing.EntityFrameworkCore.Relational.AggregateRepository, IWorldRepository
{
  public WorldRepository(IEventBus eventBus, EventContext eventContext, IEventSerializer eventSerializer)
    : base(eventBus, eventContext, eventSerializer)
  {
  }

  public Task<WorldAggregate?> LoadAsync(SlugUnit slug, CancellationToken cancellationToken)
  {
    throw new NotImplementedException(); // TODO(fpion): implement
  }

  public async Task SaveAsync(WorldAggregate world, CancellationToken cancellationToken)
  {
    await base.SaveAsync(world, cancellationToken);
  }

  public async Task SaveAsync(IEnumerable<WorldAggregate> worlds, CancellationToken cancellationToken)
  {
    await base.SaveAsync(worlds, cancellationToken);
  }
}
