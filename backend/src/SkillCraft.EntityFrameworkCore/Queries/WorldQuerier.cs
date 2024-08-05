using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Search;
using Logitar.Portal.Contracts.Users;
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
  private readonly ISqlHelper _sqlHelper;
  private readonly DbSet<WorldEntity> _worlds;

  public WorldQuerier(IActorService actorService, ISqlHelper sqlHelper, SkillCraftContext context)
  {
    _actorService = actorService;
    _sqlHelper = sqlHelper;
    _worlds = context.Worlds;
  }

  public async Task<int> CountOwnedAsync(User user, CancellationToken cancellationToken)
  {
    string ownerId = new ActorId(user.Id).Value;

    return await _worlds.AsNoTracking().Where(x => x.OwnerId == ownerId).CountAsync(cancellationToken);
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

    return world == null ? null : await MapAsync(world, cancellationToken);
  }

  public async Task<SearchResults<World>> SearchAsync(User user, SearchWorldsPayload payload, CancellationToken cancellationToken)
  {
    string ownerId = new ActorId(user.Id).Value;

    IQueryBuilder builder = _sqlHelper.QueryFrom(SkillCraftDb.Worlds.Table).SelectAll(SkillCraftDb.Worlds.Table)
      .Where(SkillCraftDb.Worlds.OwnerId, Operators.IsEqualTo(ownerId))
      .ApplyIdFilter(payload, SkillCraftDb.Worlds.Id);
    _sqlHelper.ApplyTextSearch(builder, payload.Search, SkillCraftDb.Worlds.UniqueSlug, SkillCraftDb.Worlds.DisplayName);

    IQueryable<WorldEntity> query = _worlds.FromQuery(builder).AsNoTracking();

    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<WorldEntity>? ordered = null;
    foreach (WorldSortOption sort in payload.Sort)
    {
      switch (sort.Field)
      {
        case WorldSort.DisplayName:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.DisplayName) : query.OrderBy(x => x.DisplayName))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.DisplayName) : ordered.ThenBy(x => x.DisplayName));
          break;
        case WorldSort.UniqueSlug:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UniqueSlug) : query.OrderBy(x => x.UniqueSlug))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UniqueSlug) : ordered.ThenBy(x => x.UniqueSlug));
          break;
        case WorldSort.UpdatedOn:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn));
          break;
      }
    }
    query = ordered ?? query;

    query = query.ApplyPaging(payload);

    WorldEntity[] realms = await query.ToArrayAsync(cancellationToken);
    IEnumerable<World> items = await MapAsync(realms, cancellationToken);

    return new SearchResults<World>(items, total);
  }

  private async Task<World> MapAsync(WorldEntity world, CancellationToken cancellationToken)
  {
    return (await MapAsync([world], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<World>> MapAsync(IEnumerable<WorldEntity> worlds, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = worlds.SelectMany(world => world.GetActorIds());
    IReadOnlyCollection<Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return worlds.Select(mapper.ToWorld).ToArray().AsReadOnly();
  }
}
