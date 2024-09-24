using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Search;
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
  private readonly SkillCraftContext _context;
  private readonly ISqlHelper _sqlHelper;
  private readonly DbSet<WorldEntity> _worlds;

  public WorldQuerier(IActorService actorService, SkillCraftContext context, ISqlHelper sqlHelper)
  {
    _actorService = actorService;
    _context = context;
    _sqlHelper = sqlHelper;
    _worlds = context.Worlds;
  }

  public async Task<int> CountOwnedAsync(UserId userId, CancellationToken cancellationToken)
  {
    return await _worlds.AsNoTracking().Where(x => x.OwnerId == userId.ToGuid()).CountAsync(cancellationToken);
  }

  public async Task<WorldId?> FindIdAsync(EntityKey entity, CancellationToken cancellationToken)
  {
    if (entity.Type == EntityType.World)
    {
      return new WorldId(entity.Id);
    }

    TableId table = SkillCraftDb.Helper.GetTableId(entity.Type);
    IQuery query = _sqlHelper.QueryFrom(table).Select(SkillCraftDb.Worlds.Id) // TODO(fpion): does not work, should be renamed as [Value] (SQL Server) or "Value" (PostgreSQL)
      .Join(SkillCraftDb.Worlds.WorldId, new ColumnId(SkillCraftDb.Worlds.WorldId.Name ?? string.Empty, table))
      .Where(new ColumnId("Id", table), Operators.IsEqualTo(entity.Id))
      .Build();

    Guid? id = await _context.Database.SqlQueryRaw<Guid?>(query.Text, query.Parameters.ToArray()).SingleOrDefaultAsync(cancellationToken);

    return id.HasValue ? new WorldId(id.Value) : null;
  }
  public async Task<WorldId?> FindIdAsync(Slug slug, CancellationToken cancellationToken)
  {
    string slugNormalized = SkillCraftDb.Helper.Normalize(slug);

    Guid? worldId = await _worlds.AsNoTracking()
      .Where(x => x.SlugNormalized == slugNormalized)
      .Select(x => (Guid?)x.Id)
      .SingleOrDefaultAsync(cancellationToken);

    return worldId.HasValue ? new WorldId(worldId.Value) : null;
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

  public async Task<SearchResults<WorldModel>> SearchAsync(UserId userId, SearchWorldsPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _sqlHelper.QueryFrom(SkillCraftDb.Worlds.Table).SelectAll(SkillCraftDb.Worlds.Table)
      .Where(SkillCraftDb.Worlds.OwnerId, Operators.IsEqualTo(userId.ToGuid()))
      .ApplyIdFilter(payload, SkillCraftDb.Worlds.Id);
    _sqlHelper.ApplyTextSearch(builder, payload.Search, SkillCraftDb.Worlds.Slug, SkillCraftDb.Worlds.Name);

    IQueryable<WorldEntity> query = _worlds.FromQuery(builder).AsNoTracking();

    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<WorldEntity>? ordered = null;
    foreach (WorldSortOption sort in payload.Sort)
    {
      switch (sort.Field)
      {
        case WorldSort.Name:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.Name) : ordered.ThenBy(x => x.Name));
          break;
        case WorldSort.Slug:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.Slug) : query.OrderBy(x => x.Slug))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.Slug) : ordered.ThenBy(x => x.Slug));
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

    WorldEntity[] worlds = await query.ToArrayAsync(cancellationToken);
    IEnumerable<WorldModel> items = await MapAsync(worlds, cancellationToken);

    return new SearchResults<WorldModel>(items, total);
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
