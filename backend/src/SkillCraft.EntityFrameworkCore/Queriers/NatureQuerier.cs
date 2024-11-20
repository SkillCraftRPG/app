using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Search;
using Microsoft.EntityFrameworkCore;
using SkillCraft.Application.Actors;
using SkillCraft.Application.Natures;
using SkillCraft.Contracts.Natures;
using SkillCraft.Domain.Natures;
using SkillCraft.Domain.Worlds;
using SkillCraft.EntityFrameworkCore.Entities;

namespace SkillCraft.EntityFrameworkCore.Queriers;

internal class NatureQuerier : INatureQuerier
{
  private readonly IActorService _actorService;
  private readonly DbSet<NatureEntity> _natures;
  private readonly ISqlHelper _sqlHelper;

  public NatureQuerier(IActorService actorService, SkillCraftContext context, ISqlHelper sqlHelper)
  {
    _actorService = actorService;
    _natures = context.Natures;
    _sqlHelper = sqlHelper;
  }

  public async Task<NatureModel> ReadAsync(Nature nature, CancellationToken cancellationToken)
  {
    return await ReadAsync(nature.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The nature entity 'AggregateId={nature.Id}' could not be found.");
  }
  public async Task<NatureModel?> ReadAsync(NatureId id, CancellationToken cancellationToken)
  {
    return await ReadAsync(id.WorldId, id.EntityId, cancellationToken);
  }
  public async Task<NatureModel?> ReadAsync(WorldId worldId, Guid id, CancellationToken cancellationToken)
  {
    NatureEntity? nature = await _natures.AsNoTracking()
      .Include(x => x.Gift)
      .Include(x => x.World)
      .SingleOrDefaultAsync(x => x.World!.Id == worldId.ToGuid() && x.Id == id, cancellationToken);

    return nature == null ? null : await MapAsync(nature, cancellationToken);
  }

  public async Task<SearchResults<NatureModel>> SearchAsync(WorldId worldId, SearchNaturesPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _sqlHelper.QueryFrom(SkillCraftDb.Natures.Table).SelectAll(SkillCraftDb.Natures.Table)
      .Join(SkillCraftDb.Worlds.WorldId, SkillCraftDb.Natures.WorldId)
      .LeftJoin(SkillCraftDb.Customizations.CustomizationId, SkillCraftDb.Natures.GiftId)
      .Where(SkillCraftDb.Worlds.Id, Operators.IsEqualTo(worldId.ToGuid()))
      .ApplyIdFilter(payload, SkillCraftDb.Natures.Id);
    _sqlHelper.ApplyTextSearch(builder, payload.Search, SkillCraftDb.Natures.Name);

    if (payload.Attribute.HasValue && Enum.IsDefined(payload.Attribute.Value))
    {
      builder.Where(SkillCraftDb.Natures.Attribute, Operators.IsEqualTo(payload.Attribute.Value.ToString()));
    }
    if (payload.GiftId.HasValue)
    {
      builder.Where(SkillCraftDb.Customizations.Id, Operators.IsEqualTo(payload.GiftId.Value));
    }

    IQueryable<NatureEntity> query = _natures.FromQuery(builder).AsNoTracking()
      .Include(x => x.Gift)
      .Include(x => x.World);

    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<NatureEntity>? ordered = null;
    foreach (NatureSortOption sort in payload.Sort)
    {
      switch (sort.Field)
      {
        case NatureSort.CreatedOn:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.CreatedOn) : query.OrderBy(x => x.CreatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.CreatedOn) : ordered.ThenBy(x => x.CreatedOn));
          break;
        case NatureSort.Name:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.Name) : ordered.ThenBy(x => x.Name));
          break;
        case NatureSort.UpdatedOn:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn));
          break;
      }
    }
    query = ordered ?? query;
    query = query.ApplyPaging(payload);

    NatureEntity[] natures = await query.ToArrayAsync(cancellationToken);
    IEnumerable<NatureModel> items = await MapAsync(natures, cancellationToken);

    return new SearchResults<NatureModel>(items, total);
  }

  private async Task<NatureModel> MapAsync(NatureEntity nature, CancellationToken cancellationToken)
  {
    return (await MapAsync([nature], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<NatureModel>> MapAsync(IEnumerable<NatureEntity> natures, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = natures.SelectMany(nature => nature.GetActorIds());
    IReadOnlyCollection<Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return natures.Select(mapper.ToNature).ToArray();
  }
}
