using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Search;
using Microsoft.EntityFrameworkCore;
using SkillCraft.Application.Actors;
using SkillCraft.Application.Parties;
using SkillCraft.Contracts.Parties;
using SkillCraft.Domain.Parties;
using SkillCraft.Domain.Worlds;
using SkillCraft.EntityFrameworkCore.Entities;

namespace SkillCraft.EntityFrameworkCore.Queriers;

internal class PartyQuerier : IPartyQuerier
{
  private readonly IActorService _actorService;
  private readonly DbSet<PartyEntity> _parties;
  private readonly ISqlHelper _sqlHelper;

  public PartyQuerier(IActorService actorService, SkillCraftContext context, ISqlHelper sqlHelper)
  {
    _actorService = actorService;
    _parties = context.Parties;
    _sqlHelper = sqlHelper;
  }

  public async Task<PartyModel> ReadAsync(Party party, CancellationToken cancellationToken)
  {
    return await ReadAsync(party.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The party entity 'AggregateId={party.Id}' could not be found.");
  }
  public async Task<PartyModel?> ReadAsync(PartyId id, CancellationToken cancellationToken)
  {
    return await ReadAsync(id.WorldId, id.EntityId, cancellationToken);
  }
  public async Task<PartyModel?> ReadAsync(WorldId worldId, Guid id, CancellationToken cancellationToken)
  {
    PartyEntity? party = await _parties.AsNoTracking()
      .Include(x => x.World)
      .SingleOrDefaultAsync(x => x.World!.Id == worldId.ToGuid() && x.Id == id, cancellationToken);

    return party == null ? null : await MapAsync(party, cancellationToken);
  }

  public async Task<SearchResults<PartyModel>> SearchAsync(WorldId worldId, SearchPartiesPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _sqlHelper.QueryFrom(SkillCraftDb.Parties.Table).SelectAll(SkillCraftDb.Parties.Table)
      .Join(SkillCraftDb.Worlds.WorldId, SkillCraftDb.Parties.WorldId)
      .Where(SkillCraftDb.Worlds.Id, Operators.IsEqualTo(worldId.ToGuid()))
      .ApplyIdFilter(payload, SkillCraftDb.Parties.Id);
    _sqlHelper.ApplyTextSearch(builder, payload.Search, SkillCraftDb.Parties.Name);

    IQueryable<PartyEntity> query = _parties.FromQuery(builder).AsNoTracking()
      .Include(x => x.World);

    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<PartyEntity>? ordered = null;
    foreach (PartySortOption sort in payload.Sort)
    {
      switch (sort.Field)
      {
        case PartySort.CreatedOn:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.CreatedOn) : query.OrderBy(x => x.CreatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.CreatedOn) : ordered.ThenBy(x => x.CreatedOn));
          break;
        case PartySort.Name:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.Name) : ordered.ThenBy(x => x.Name));
          break;
        case PartySort.UpdatedOn:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn));
          break;
      }
    }
    query = ordered ?? query;
    query = query.ApplyPaging(payload);

    PartyEntity[] parties = await query.ToArrayAsync(cancellationToken);
    IEnumerable<PartyModel> items = await MapAsync(parties, cancellationToken);

    return new SearchResults<PartyModel>(items, total);
  }

  private async Task<PartyModel> MapAsync(PartyEntity party, CancellationToken cancellationToken)
  {
    return (await MapAsync([party], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<PartyModel>> MapAsync(IEnumerable<PartyEntity> parties, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = parties.SelectMany(party => party.GetActorIds());
    IReadOnlyCollection<Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return parties.Select(mapper.ToParty).ToArray();
  }
}
