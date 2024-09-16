﻿using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Search;
using Microsoft.EntityFrameworkCore;
using SkillCraft.Application.Actors;
using SkillCraft.Application.Personalities;
using SkillCraft.Contracts.Personalities;
using SkillCraft.Domain.Personalities;
using SkillCraft.Domain.Worlds;
using SkillCraft.EntityFrameworkCore.Entities;

namespace SkillCraft.EntityFrameworkCore.Queriers;

internal class PersonalityQuerier : IPersonalityQuerier
{
  private readonly IActorService _actorService;
  private readonly DbSet<PersonalityEntity> _personalities;
  private readonly ISqlHelper _sqlHelper;

  public PersonalityQuerier(IActorService actorService, SkillCraftContext context, ISqlHelper sqlHelper)
  {
    _actorService = actorService;
    _personalities = context.Personalities;
    _sqlHelper = sqlHelper;
  }

  public async Task<PersonalityModel> ReadAsync(Personality personality, CancellationToken cancellationToken)
  {
    return await ReadAsync(personality.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The personality entity 'Id={personality.Id.ToGuid()}' could not be found.");
  }
  public async Task<PersonalityModel?> ReadAsync(PersonalityId id, CancellationToken cancellationToken)
  {
    return await ReadAsync(id.ToGuid(), cancellationToken);
  }
  public async Task<PersonalityModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    PersonalityEntity? personality = await _personalities.AsNoTracking()
      .Include(x => x.World)
      .Include(x => x.Gift)
      .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    return personality == null ? null : await MapAsync(personality, cancellationToken);
  }

  public async Task<SearchResults<PersonalityModel>> SearchAsync(WorldId worldId, SearchPersonalitiesPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _sqlHelper.QueryFrom(SkillCraftDb.Personalities.Table).SelectAll(SkillCraftDb.Personalities.Table)
      .Join(SkillCraftDb.Worlds.WorldId, SkillCraftDb.Personalities.WorldId)
      .Where(SkillCraftDb.Worlds.Id, Operators.IsEqualTo(worldId.ToGuid()))
      .ApplyIdFilter(payload, SkillCraftDb.Personalities.Id);
    _sqlHelper.ApplyTextSearch(builder, payload.Search, SkillCraftDb.Personalities.Name);

    IQueryable<PersonalityEntity> query = _personalities.FromQuery(builder).AsNoTracking()
      .Include(x => x.World)
      .Include(x => x.Gift);

    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<PersonalityEntity>? ordered = null;
    foreach (PersonalitySortOption sort in payload.Sort)
    {
      switch (sort.Field)
      {
        case PersonalitySort.Name:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.Name) : ordered.ThenBy(x => x.Name));
          break;
        case PersonalitySort.UpdatedOn:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn));
          break;
      }
    }
    query = ordered ?? query;
    query = query.ApplyPaging(payload);

    PersonalityEntity[] personalities = await query.ToArrayAsync(cancellationToken);
    IEnumerable<PersonalityModel> items = await MapAsync(personalities, cancellationToken);

    return new SearchResults<PersonalityModel>(items, total);
  }

  private async Task<PersonalityModel> MapAsync(PersonalityEntity personality, CancellationToken cancellationToken)
  {
    return (await MapAsync([personality], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<PersonalityModel>> MapAsync(IEnumerable<PersonalityEntity> personalities, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = personalities.SelectMany(personality => personality.GetActorIds());
    IReadOnlyCollection<Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return personalities.Select(mapper.ToPersonality).ToArray();
  }
}