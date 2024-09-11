using Logitar.EventSourcing;
using Logitar.Portal.Contracts.Actors;
using Microsoft.EntityFrameworkCore;
using SkillCraft.Application.Actors;
using SkillCraft.Application.Worlds;
using SkillCraft.Contracts.Worlds;
using SkillCraft.Domain;
using SkillCraft.Domain.Worlds;
using SkillCraft.EntityFrameworkCore.Entities;

namespace SkillCraft.EntityFrameworkCore.Queriers;

internal class WorldQuerier : IWorldQuerier
{
  private readonly IActorService _actorService;
  private readonly DbSet<WorldEntity> _worlds;

  public WorldQuerier(IActorService actorService, SkillCraftContext context)
  {
    _actorService = actorService;
    _worlds = context.Worlds;
  }

  public async Task<int> CountOwnedAsync(UserId userId, CancellationToken cancellationToken)
  {
    return await _worlds.AsNoTracking().Where(x => x.OwnerId == userId.ToGuid()).CountAsync(cancellationToken);
  }

  public async Task<WorldModel> ReadAsync(World world, CancellationToken cancellationToken)
  {
    return await ReadAsync(world.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The world entity 'Id={world.Id.ToGuid()}' could not be found.");
  }
  public async Task<WorldModel?> ReadAsync(WorldId id, CancellationToken cancellationToken)
  {
    return await ReadAsync(id.ToGuid(), cancellationToken);
  }
  public async Task<WorldModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    WorldEntity? world = await _worlds.AsNoTracking()
      .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    return world == null ? null : await MapAsync(world, cancellationToken);
  }

  public async Task<WorldModel?> ReadAsync(string slug, CancellationToken cancellationToken)
  {
    string slugNormalized = SkillCraftDb.Helper.Normalize(slug);

    WorldEntity? world = await _worlds.AsNoTracking()
      .SingleOrDefaultAsync(x => x.SlugNormalized == slugNormalized, cancellationToken);

    return world == null ? null : await MapAsync(world, cancellationToken);
  }

  private async Task<WorldModel> MapAsync(WorldEntity world, CancellationToken cancellationToken)
  {
    return (await MapAsync([world], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<WorldModel>> MapAsync(IEnumerable<WorldEntity> worlds, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = worlds.SelectMany(world => world.GetActorIds());
    IReadOnlyCollection<Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return worlds.Select(mapper.ToWorld).ToArray();
  }
}
