using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Search;
using Microsoft.EntityFrameworkCore;
using SkillCraft.Application.Actors;
using SkillCraft.Application.Lineages;
using SkillCraft.Contracts.Lineages;
using SkillCraft.Domain.Lineages;
using SkillCraft.Domain.Worlds;
using SkillCraft.EntityFrameworkCore.Entities;
using Attribute = SkillCraft.Contracts.Attribute;

namespace SkillCraft.EntityFrameworkCore.Queriers;

internal class LineageQuerier : ILineageQuerier
{
  private readonly IActorService _actorService;
  private readonly DbSet<LineageEntity> _lineages;
  private readonly ISqlHelper _sqlHelper;

  public LineageQuerier(IActorService actorService, SkillCraftContext context, ISqlHelper sqlHelper)
  {
    _actorService = actorService;
    _lineages = context.Lineages;
    _sqlHelper = sqlHelper;
  }

  public async Task<LineageModel> ReadAsync(Lineage lineage, CancellationToken cancellationToken)
  {
    return await ReadAsync(lineage.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The lineage entity 'AggregateId={lineage.Id}' could not be found.");
  }
  public async Task<LineageModel?> ReadAsync(LineageId id, CancellationToken cancellationToken)
  {
    return await ReadAsync(id.WorldId, id.EntityId, cancellationToken);
  }
  public async Task<LineageModel?> ReadAsync(WorldId worldId, Guid id, CancellationToken cancellationToken)
  {
    LineageEntity? lineage = await _lineages.AsNoTracking()
      .Include(x => x.Languages).ThenInclude(x => x.World)
      .Include(x => x.World)
      .SingleOrDefaultAsync(x => x.World!.Id == worldId.ToGuid() && x.Id == id, cancellationToken);

    return lineage == null ? null : await MapAsync(lineage, cancellationToken);
  }

  public async Task<SearchResults<LineageModel>> SearchAsync(WorldId worldId, SearchLineagesPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _sqlHelper.QueryFrom(SkillCraftDb.Lineages.Table).SelectAll(SkillCraftDb.Lineages.Table)
      .Join(SkillCraftDb.Worlds.WorldId, SkillCraftDb.Lineages.WorldId)
      .Where(SkillCraftDb.Worlds.Id, Operators.IsEqualTo(worldId.ToGuid()))
      .ApplyIdFilter(payload, SkillCraftDb.Lineages.Id);
    _sqlHelper.ApplyTextSearch(builder, payload.Search, SkillCraftDb.Lineages.Name);

    if (payload.ParentId.HasValue)
    {
      TableId parent = new(SkillCraftDb.Lineages.Table.Schema, SkillCraftDb.Lineages.Table.Table ?? string.Empty, "Parent");
      builder.Join(
        new ColumnId(SkillCraftDb.Lineages.LineageId.Name ?? string.Empty, parent),
        SkillCraftDb.Lineages.ParentId,
        new OperatorCondition(
          new ColumnId(SkillCraftDb.Lineages.Id.Name ?? string.Empty, parent),
          Operators.IsEqualTo(payload.ParentId.Value)));
    }
    else
    {
      builder.Where(SkillCraftDb.Lineages.ParentId, Operators.IsNull());
    }

    if (payload.Attribute.HasValue)
    {
      ColumnId? column = null;
      switch (payload.Attribute.Value)
      {
        case Attribute.Agility:
          column = SkillCraftDb.Lineages.Agility;
          break;
        case Attribute.Coordination:
          column = SkillCraftDb.Lineages.Coordination;
          break;
        case Attribute.Intellect:
          column = SkillCraftDb.Lineages.Intellect;
          break;
        case Attribute.Presence:
          column = SkillCraftDb.Lineages.Presence;
          break;
        case Attribute.Sensitivity:
          column = SkillCraftDb.Lineages.Sensitivity;
          break;
        case Attribute.Spirit:
          column = SkillCraftDb.Lineages.Spirit;
          break;
        case Attribute.Vigor:
          column = SkillCraftDb.Lineages.Vigor;
          break;
      }
      if (column != null)
      {
        builder.Where(column, Operators.IsGreaterThan(0));
      }
    }
    if (payload.LanguageId.HasValue)
    {
      builder.Join(SkillCraftDb.LineageLanguages.LineageId, SkillCraftDb.Lineages.LineageId)
        .Join(SkillCraftDb.Languages.LanguageId, SkillCraftDb.LineageLanguages.LanguageId)
        .Where(SkillCraftDb.Languages.Id, Operators.IsEqualTo(payload.LanguageId.Value));
    }
    if (payload.SizeCategory.HasValue && Enum.IsDefined(payload.SizeCategory.Value))
    {
      builder.Where(SkillCraftDb.Lineages.SizeCategory, Operators.IsEqualTo(payload.SizeCategory.Value.ToString()));
    }

    IQueryable<LineageEntity> query = _lineages.FromQuery(builder).AsNoTracking()
      .Include(x => x.Languages).ThenInclude(x => x.World)
      .Include(x => x.World);

    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<LineageEntity>? ordered = null;
    foreach (LineageSortOption sort in payload.Sort)
    {
      switch (sort.Field)
      {
        case LineageSort.CreatedOn:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.CreatedOn) : query.OrderBy(x => x.CreatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.CreatedOn) : ordered.ThenBy(x => x.CreatedOn));
          break;
        case LineageSort.Name:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.Name) : ordered.ThenBy(x => x.Name));
          break;
        case LineageSort.UpdatedOn:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn));
          break;
      }
    }
    query = ordered ?? query;
    query = query.ApplyPaging(payload);

    LineageEntity[] lineages = await query.ToArrayAsync(cancellationToken);
    IEnumerable<LineageModel> items = await MapAsync(lineages, cancellationToken);

    return new SearchResults<LineageModel>(items, total);
  }

  private async Task<LineageModel> MapAsync(LineageEntity lineage, CancellationToken cancellationToken)
  {
    return (await MapAsync([lineage], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<LineageModel>> MapAsync(IEnumerable<LineageEntity> lineages, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = lineages.SelectMany(lineage => lineage.GetActorIds());
    IReadOnlyCollection<Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return lineages.Select(mapper.ToLineage).ToArray();
  }
}
