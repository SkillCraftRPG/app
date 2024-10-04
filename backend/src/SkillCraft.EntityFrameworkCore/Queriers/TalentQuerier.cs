using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Search;
using Microsoft.EntityFrameworkCore;
using SkillCraft.Application.Actors;
using SkillCraft.Application.Talents;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Talents;
using SkillCraft.Domain.Talents;
using SkillCraft.Domain.Worlds;
using SkillCraft.EntityFrameworkCore.Entities;

namespace SkillCraft.EntityFrameworkCore.Queriers;

internal class TalentQuerier : ITalentQuerier
{
  private readonly IActorService _actorService;
  private readonly DbSet<TalentEntity> _talents;
  private readonly ISqlHelper _sqlHelper;

  public TalentQuerier(IActorService actorService, SkillCraftContext context, ISqlHelper sqlHelper)
  {
    _actorService = actorService;
    _talents = context.Talents;
    _sqlHelper = sqlHelper;
  }

  public async Task<TalentId?> FindIdAsync(WorldId worldId, Skill skill, CancellationToken cancellationToken)
  {
    Guid? talentId = await _talents.AsNoTracking()
      .Where(x => x.World!.Id == worldId.ToGuid() && x.Skill == skill)
      .Select(x => (Guid?)x.Id)
      .SingleOrDefaultAsync(cancellationToken);

    return talentId.HasValue ? new TalentId(worldId, talentId.Value) : null;
  }

  public async Task<TalentModel> ReadAsync(Talent talent, CancellationToken cancellationToken)
  {
    return await ReadAsync(talent.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The talent entity 'AggregateId={talent.Id}' could not be found.");
  }
  public async Task<TalentModel?> ReadAsync(TalentId id, CancellationToken cancellationToken)
  {
    return await ReadAsync(id.WorldId, id.EntityId, cancellationToken);
  }
  public async Task<TalentModel?> ReadAsync(WorldId worldId, Guid id, CancellationToken cancellationToken)
  {
    TalentEntity? talent = await _talents.AsNoTracking()
      .Include(x => x.World)
      .SingleOrDefaultAsync(x => x.World!.Id == worldId.ToGuid() && x.Id == id, cancellationToken);

    return talent == null ? null : await MapAsync(talent, cancellationToken);
  }

  public async Task<SearchResults<TalentModel>> SearchAsync(WorldId worldId, SearchTalentsPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _sqlHelper.QueryFrom(SkillCraftDb.Talents.Table).SelectAll(SkillCraftDb.Talents.Table)
      .Join(SkillCraftDb.Worlds.WorldId, SkillCraftDb.Talents.WorldId)
      .Where(SkillCraftDb.Worlds.Id, Operators.IsEqualTo(worldId.ToGuid()))
      .ApplyIdFilter(payload, SkillCraftDb.Talents.Id);
    _sqlHelper.ApplyTextSearch(builder, payload.Search, SkillCraftDb.Talents.Name);

    if (payload.AllowMultiplePurchases.HasValue)
    {
      builder.Where(SkillCraftDb.Talents.AllowMultiplePurchases, Operators.IsEqualTo(payload.AllowMultiplePurchases.Value));
    }
    if (payload.HasSkill.HasValue)
    {
      builder.Where(SkillCraftDb.Talents.Skill, payload.HasSkill.Value ? Operators.IsNotNull() : Operators.IsNull());
    }
    if (payload.RequiredTalentId.HasValue)
    {
      TableId required = new(SkillCraftDb.Talents.Table.Schema, SkillCraftDb.Talents.Table.Table ?? string.Empty, "Required");
      builder.Join(
        new ColumnId(SkillCraftDb.Talents.TalentId.Name ?? string.Empty, required),
        SkillCraftDb.Talents.RequiredTalentId,
        new OperatorCondition(
          new ColumnId(SkillCraftDb.Talents.Id.Name ?? string.Empty, required),
          Operators.IsEqualTo(payload.RequiredTalentId.Value)));
    }
    if (payload.Tier != null && payload.Tier.Values.Count > 0)
    {
      builder.Where(SkillCraftDb.Talents.Tier, GetTierOperator(payload.Tier));
    }

    IQueryable<TalentEntity> query = _talents.FromQuery(builder).AsNoTracking()
      .Include(x => x.World);

    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<TalentEntity>? ordered = null;
    foreach (TalentSortOption sort in payload.Sort)
    {
      switch (sort.Field)
      {
        case TalentSort.CreatedOn:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.CreatedOn) : query.OrderBy(x => x.CreatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.CreatedOn) : ordered.ThenBy(x => x.CreatedOn));
          break;
        case TalentSort.Name:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.Name) : ordered.ThenBy(x => x.Name));
          break;
        case TalentSort.UpdatedOn:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn));
          break;
      }
    }
    query = ordered ?? query;
    query = query.ApplyPaging(payload);

    TalentEntity[] talents = await query.ToArrayAsync(cancellationToken);
    IEnumerable<TalentModel> items = await MapAsync(talents, cancellationToken);

    return new SearchResults<TalentModel>(items, total);
  }

  private async Task<TalentModel> MapAsync(TalentEntity talent, CancellationToken cancellationToken)
  {
    return (await MapAsync([talent], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<TalentModel>> MapAsync(IEnumerable<TalentEntity> talents, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = talents.SelectMany(talent => talent.GetActorIds());
    IReadOnlyCollection<Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return talents.Select(mapper.ToTalent).ToArray();
  }

  private static ConditionalOperator GetTierOperator(TierFilter tier)
  {
    return tier.Operator.Trim().ToLowerInvariant() switch
    {
      "gt" => Operators.IsGreaterThan(tier.Values.First()),
      "gte" => Operators.IsGreaterThanOrEqualTo(tier.Values.First()),
      "in" => Operators.IsIn(tier.Values.Distinct().Select(value => (object)value).ToArray()),
      "lt" => Operators.IsLessThan(tier.Values.First()),
      "lte" => Operators.IsLessThanOrEqualTo(tier.Values.First()),
      "ne" => Operators.IsNotEqualTo(tier.Values.First()),
      "nin" => Operators.IsNotIn(tier.Values.Distinct().Select(value => (object)value).ToArray()),
      _ => Operators.IsEqualTo(tier.Values.First()),
    };
  }
}
