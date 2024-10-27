using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Search;
using Microsoft.EntityFrameworkCore;
using SkillCraft.Application.Actors;
using SkillCraft.Application.Items;
using SkillCraft.Contracts.Items;
using SkillCraft.Domain.Items;
using SkillCraft.Domain.Worlds;
using SkillCraft.EntityFrameworkCore.Entities;

namespace SkillCraft.EntityFrameworkCore.Queriers;

internal class ItemQuerier : IItemQuerier
{
  private readonly IActorService _actorService;
  private readonly DbSet<ItemEntity> _items;
  private readonly ISqlHelper _sqlHelper;

  public ItemQuerier(IActorService actorService, SkillCraftContext context, ISqlHelper sqlHelper)
  {
    _actorService = actorService;
    _items = context.Items;
    _sqlHelper = sqlHelper;
  }

  public async Task<ItemModel> ReadAsync(Item item, CancellationToken cancellationToken)
  {
    return await ReadAsync(item.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The item entity 'AggregateId={item.Id}' could not be found.");
  }
  public async Task<ItemModel?> ReadAsync(ItemId id, CancellationToken cancellationToken)
  {
    return await ReadAsync(id.WorldId, id.EntityId, cancellationToken);
  }
  public async Task<ItemModel?> ReadAsync(WorldId worldId, Guid id, CancellationToken cancellationToken)
  {
    ItemEntity? item = await _items.AsNoTracking()
      .Include(x => x.World)
      .SingleOrDefaultAsync(x => x.World!.Id == worldId.ToGuid() && x.Id == id, cancellationToken);

    return item == null ? null : await MapAsync(item, cancellationToken);
  }

  public async Task<SearchResults<ItemModel>> SearchAsync(WorldId worldId, SearchItemsPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _sqlHelper.QueryFrom(SkillCraftDb.Items.Table).SelectAll(SkillCraftDb.Items.Table)
      .Join(SkillCraftDb.Worlds.WorldId, SkillCraftDb.Items.WorldId)
      .Where(SkillCraftDb.Worlds.Id, Operators.IsEqualTo(worldId.ToGuid()))
      .ApplyIdFilter(payload, SkillCraftDb.Items.Id);
    _sqlHelper.ApplyTextSearch(builder, payload.Search, SkillCraftDb.Items.Name);

    if (payload.Category.HasValue && Enum.IsDefined(payload.Category.Value))
    {
      builder.Where(SkillCraftDb.Items.Category, Operators.IsEqualTo(payload.Category.Value.ToString()));
    }

    IQueryable<ItemEntity> query = _items.FromQuery(builder).AsNoTracking()
      .Include(x => x.World);

    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<ItemEntity>? ordered = null;
    foreach (ItemSortOption sort in payload.Sort)
    {
      switch (sort.Field)
      {
        case ItemSort.CreatedOn:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.CreatedOn) : query.OrderBy(x => x.CreatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.CreatedOn) : ordered.ThenBy(x => x.CreatedOn));
          break;
        case ItemSort.Name:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.Name) : ordered.ThenBy(x => x.Name));
          break;
        case ItemSort.UpdatedOn:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn));
          break;
      }
    }
    query = ordered ?? query;
    query = query.ApplyPaging(payload);

    ItemEntity[] entities = await query.ToArrayAsync(cancellationToken);
    IEnumerable<ItemModel> items = await MapAsync(entities, cancellationToken);

    return new SearchResults<ItemModel>(items, total);
  }

  private async Task<ItemModel> MapAsync(ItemEntity item, CancellationToken cancellationToken)
  {
    return (await MapAsync([item], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<ItemModel>> MapAsync(IEnumerable<ItemEntity> items, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = items.SelectMany(item => item.GetActorIds());
    IReadOnlyCollection<Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return items.Select(mapper.ToItem).ToArray();
  }
}
