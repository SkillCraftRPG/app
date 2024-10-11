using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Search;
using Microsoft.EntityFrameworkCore;
using SkillCraft.Application.Actors;
using SkillCraft.Application.Castes;
using SkillCraft.Contracts.Castes;
using SkillCraft.Domain.Castes;
using SkillCraft.Domain.Worlds;
using SkillCraft.EntityFrameworkCore.Entities;

namespace SkillCraft.EntityFrameworkCore.Queriers;

internal class CasteQuerier : ICasteQuerier
{
  private readonly IActorService _actorService;
  private readonly DbSet<CasteEntity> _castes;
  private readonly ISqlHelper _sqlHelper;

  public CasteQuerier(IActorService actorService, SkillCraftContext context, ISqlHelper sqlHelper)
  {
    _actorService = actorService;
    _castes = context.Castes;
    _sqlHelper = sqlHelper;
  }

  public async Task<CasteModel> ReadAsync(Caste caste, CancellationToken cancellationToken)
  {
    return await ReadAsync(caste.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The caste entity 'AggregateId={caste.Id}' could not be found.");
  }
  public async Task<CasteModel?> ReadAsync(CasteId id, CancellationToken cancellationToken)
  {
    return await ReadAsync(id.WorldId, id.EntityId, cancellationToken);
  }
  public async Task<CasteModel?> ReadAsync(WorldId worldId, Guid id, CancellationToken cancellationToken)
  {
    CasteEntity? caste = await _castes.AsNoTracking()
      .Include(x => x.World)
      .SingleOrDefaultAsync(x => x.World!.Id == worldId.ToGuid() && x.Id == id, cancellationToken);

    return caste == null ? null : await MapAsync(caste, cancellationToken);
  }

  public async Task<SearchResults<CasteModel>> SearchAsync(WorldId worldId, SearchCastesPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _sqlHelper.QueryFrom(SkillCraftDb.Castes.Table).SelectAll(SkillCraftDb.Castes.Table)
      .Join(SkillCraftDb.Worlds.WorldId, SkillCraftDb.Castes.WorldId)
      .Where(SkillCraftDb.Worlds.Id, Operators.IsEqualTo(worldId.ToGuid()))
      .ApplyIdFilter(payload, SkillCraftDb.Castes.Id);
    _sqlHelper.ApplyTextSearch(builder, payload.Search, SkillCraftDb.Castes.Name);

    if (payload.Skill.HasValue && Enum.IsDefined(payload.Skill.Value))
    {
      builder.Where(SkillCraftDb.Castes.Skill, Operators.IsEqualTo(payload.Skill.Value.ToString()));
    }

    IQueryable<CasteEntity> query = _castes.FromQuery(builder).AsNoTracking()
      .Include(x => x.World);

    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<CasteEntity>? ordered = null;
    foreach (CasteSortOption sort in payload.Sort)
    {
      switch (sort.Field)
      {
        case CasteSort.CreatedOn:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.CreatedOn) : query.OrderBy(x => x.CreatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.CreatedOn) : ordered.ThenBy(x => x.CreatedOn));
          break;
        case CasteSort.Name:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.Name) : ordered.ThenBy(x => x.Name));
          break;
        case CasteSort.UpdatedOn:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn));
          break;
      }
    }
    query = ordered ?? query;
    query = query.ApplyPaging(payload);

    CasteEntity[] castes = await query.ToArrayAsync(cancellationToken);
    IEnumerable<CasteModel> items = await MapAsync(castes, cancellationToken);

    return new SearchResults<CasteModel>(items, total);
  }

  private async Task<CasteModel> MapAsync(CasteEntity caste, CancellationToken cancellationToken)
  {
    return (await MapAsync([caste], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<CasteModel>> MapAsync(IEnumerable<CasteEntity> castes, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = castes.SelectMany(caste => caste.GetActorIds());
    IReadOnlyCollection<Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return castes.Select(mapper.ToCaste).ToArray();
  }
}
