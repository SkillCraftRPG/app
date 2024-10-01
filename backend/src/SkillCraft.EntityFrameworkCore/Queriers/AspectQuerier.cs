using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Search;
using Microsoft.EntityFrameworkCore;
using SkillCraft.Application.Actors;
using SkillCraft.Application.Aspects;
using SkillCraft.Contracts.Aspects;
using SkillCraft.Domain.Aspects;
using SkillCraft.Domain.Worlds;
using SkillCraft.EntityFrameworkCore.Entities;

namespace SkillCraft.EntityFrameworkCore.Queriers;

internal class AspectQuerier : IAspectQuerier
{
  private readonly IActorService _actorService;
  private readonly DbSet<AspectEntity> _aspects;
  private readonly ISqlHelper _sqlHelper;

  public AspectQuerier(IActorService actorService, SkillCraftContext context, ISqlHelper sqlHelper)
  {
    _actorService = actorService;
    _aspects = context.Aspects;
    _sqlHelper = sqlHelper;
  }

  public async Task<AspectModel> ReadAsync(Aspect aspect, CancellationToken cancellationToken)
  {
    return await ReadAsync(aspect.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The aspect entity 'AggregateId={aspect.Id}' could not be found.");
  }
  public async Task<AspectModel?> ReadAsync(AspectId id, CancellationToken cancellationToken)
  {
    return await ReadAsync(id.WorldId, id.EntityId, cancellationToken);
  }
  public async Task<AspectModel?> ReadAsync(WorldId worldId, Guid id, CancellationToken cancellationToken)
  {
    AspectEntity? aspect = await _aspects.AsNoTracking()
      .Include(x => x.World)
      .SingleOrDefaultAsync(x => x.World!.Id == worldId.ToGuid() && x.Id == id, cancellationToken);

    return aspect == null ? null : await MapAsync(aspect, cancellationToken);
  }

  public async Task<SearchResults<AspectModel>> SearchAsync(WorldId worldId, SearchAspectsPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _sqlHelper.QueryFrom(SkillCraftDb.Aspects.Table).SelectAll(SkillCraftDb.Aspects.Table)
      .Join(SkillCraftDb.Worlds.WorldId, SkillCraftDb.Aspects.WorldId)
      .Where(SkillCraftDb.Worlds.Id, Operators.IsEqualTo(worldId.ToGuid()))
      .ApplyIdFilter(payload, SkillCraftDb.Aspects.Id);
    _sqlHelper.ApplyTextSearch(builder, payload.Search, SkillCraftDb.Aspects.Name);

    if (payload.Attribute.HasValue)
    {
      ColumnId[] columns = [SkillCraftDb.Aspects.MandatoryAttribute1, SkillCraftDb.Aspects.MandatoryAttribute2, SkillCraftDb.Aspects.OptionalAttribute1, SkillCraftDb.Aspects.OptionalAttribute2];
      builder.WhereOr(columns.Select(column => new OperatorCondition(column, Operators.IsEqualTo(payload.Attribute.Value.ToString()))).ToArray());
    }
    if (payload.Skill.HasValue)
    {
      ColumnId[] columns = [SkillCraftDb.Aspects.DiscountedSkill1, SkillCraftDb.Aspects.DiscountedSkill2];
      builder.WhereOr(columns.Select(column => new OperatorCondition(column, Operators.IsEqualTo(payload.Skill.Value.ToString()))).ToArray());
    }

    IQueryable<AspectEntity> query = _aspects.FromQuery(builder).AsNoTracking()
      .Include(x => x.World);

    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<AspectEntity>? ordered = null;
    foreach (AspectSortOption sort in payload.Sort)
    {
      switch (sort.Field)
      {
        case AspectSort.CreatedOn:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.CreatedOn) : query.OrderBy(x => x.CreatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.CreatedOn) : ordered.ThenBy(x => x.CreatedOn));
          break;
        case AspectSort.Name:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.Name) : ordered.ThenBy(x => x.Name));
          break;
        case AspectSort.UpdatedOn:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn));
          break;
      }
    }
    query = ordered ?? query;
    query = query.ApplyPaging(payload);

    AspectEntity[] aspects = await query.ToArrayAsync(cancellationToken);
    IEnumerable<AspectModel> items = await MapAsync(aspects, cancellationToken);

    return new SearchResults<AspectModel>(items, total);
  }

  private async Task<AspectModel> MapAsync(AspectEntity aspect, CancellationToken cancellationToken)
  {
    return (await MapAsync([aspect], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<AspectModel>> MapAsync(IEnumerable<AspectEntity> aspects, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = aspects.SelectMany(aspect => aspect.GetActorIds());
    IReadOnlyCollection<Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return aspects.Select(mapper.ToAspect).ToArray();
  }
}
