﻿using Logitar.Data;
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

    return talentId.HasValue ? new TalentId(talentId.Value) : null;
  }

  public async Task<TalentModel> ReadAsync(Talent talent, CancellationToken cancellationToken)
  {
    return await ReadAsync(talent.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The talent entity 'Id={talent.Id.ToGuid()}' could not be found.");
  }
  public async Task<TalentModel?> ReadAsync(TalentId id, CancellationToken cancellationToken)
  {
    return await ReadAsync(id.ToGuid(), cancellationToken);
  }
  public async Task<TalentModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    TalentEntity? talent = await _talents.AsNoTracking()
      .Include(x => x.World)
      .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    return talent == null ? null : await MapAsync(talent, cancellationToken);
  }

  public async Task<SearchResults<TalentModel>> SearchAsync(WorldId worldId, SearchTalentsPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _sqlHelper.QueryFrom(SkillCraftDb.Talents.Table).SelectAll(SkillCraftDb.Talents.Table)
      .Join(SkillCraftDb.Worlds.WorldId, SkillCraftDb.Talents.WorldId)
      .Where(SkillCraftDb.Worlds.Id, Operators.IsEqualTo(worldId.ToGuid()))
      .ApplyIdFilter(payload, SkillCraftDb.Talents.Id);
    _sqlHelper.ApplyTextSearch(builder, payload.Search, SkillCraftDb.Talents.Name);

    IQueryable<TalentEntity> query = _talents.FromQuery(builder).AsNoTracking()
      .Include(x => x.World);

    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<TalentEntity>? ordered = null;
    foreach (TalentSortOption sort in payload.Sort)
    {
      switch (sort.Field)
      {
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
}
