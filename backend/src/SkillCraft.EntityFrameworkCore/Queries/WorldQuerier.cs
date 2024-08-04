using Microsoft.EntityFrameworkCore;
using SkillCraft.Application.Actors;
using SkillCraft.Application.Worlds;
using SkillCraft.Contracts.Worlds;
using SkillCraft.Domain.Worlds;
using SkillCraft.EntityFrameworkCore.Entities;

namespace SkillCraft.EntityFrameworkCore.Queries;

internal class WorldQuerier : IWorldQuerier
{
  private readonly IActorService _actorService;
  private readonly DbSet<WorldEntity> _worlds;

  public WorldQuerier(IActorService actorService, SkillCraftContext context)
  {
    _actorService = actorService;
    _worlds = context.Worlds;
  }

  public async Task<World> ReadAsync(WorldAggregate world, CancellationToken cancellationToken)
  {
    return await ReadAsync(world.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The world entity 'AggregateId={world.Id.AggregateId}' could not be found.");
  }
  public async Task<World?> ReadAsync(WorldId id, CancellationToken cancellationToken)
  {
    return await ReadAsync(id.ToGuid(), cancellationToken);
  }
  public async Task<World?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    WorldEntity? world = await _worlds.AsNoTracking()
      .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    throw new NotImplementedException(); // TODO(fpion): implement
  }
}
