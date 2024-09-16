using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Search;
using Microsoft.EntityFrameworkCore;
using SkillCraft.Application.Actors;
using SkillCraft.Application.Customizations;
using SkillCraft.Contracts.Customizations;
using SkillCraft.Domain.Customizations;
using SkillCraft.Domain.Worlds;
using SkillCraft.EntityFrameworkCore.Entities;

namespace SkillCraft.EntityFrameworkCore.Queriers;

internal class CustomizationQuerier : ICustomizationQuerier
{
  private readonly IActorService _actorService;
  private readonly DbSet<CustomizationEntity> _customizations;
  private readonly ISqlHelper _sqlHelper;

  public CustomizationQuerier(IActorService actorService, SkillCraftContext context, ISqlHelper sqlHelper)
  {
    _actorService = actorService;
    _customizations = context.Customizations;
    _sqlHelper = sqlHelper;
  }

  public async Task<CustomizationModel> ReadAsync(Customization customization, CancellationToken cancellationToken)
  {
    return await ReadAsync(customization.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The customization entity 'Id={customization.Id.ToGuid()}' could not be found.");
  }
  public async Task<CustomizationModel?> ReadAsync(CustomizationId id, CancellationToken cancellationToken)
  {
    return await ReadAsync(id.ToGuid(), cancellationToken);
  }
  public async Task<CustomizationModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    CustomizationEntity? customization = await _customizations.AsNoTracking()
      .Include(x => x.World)
      .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    return customization == null ? null : await MapAsync(customization, cancellationToken);
  }

  public async Task<SearchResults<CustomizationModel>> SearchAsync(WorldId worldId, SearchCustomizationsPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _sqlHelper.QueryFrom(SkillCraftDb.Customizations.Table).SelectAll(SkillCraftDb.Customizations.Table)
      .Join(SkillCraftDb.Worlds.WorldId, SkillCraftDb.Customizations.WorldId)
      .Where(SkillCraftDb.Worlds.Id, Operators.IsEqualTo(worldId.ToGuid()))
      .ApplyIdFilter(payload, SkillCraftDb.Customizations.Id);
    _sqlHelper.ApplyTextSearch(builder, payload.Search, SkillCraftDb.Customizations.Name);

    if (payload.Type.HasValue)
    {
      builder.Where(SkillCraftDb.Customizations.Type, Operators.IsEqualTo(payload.Type.Value.ToString()));
    }

    IQueryable<CustomizationEntity> query = _customizations.FromQuery(builder).AsNoTracking()
      .Include(x => x.World);

    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<CustomizationEntity>? ordered = null;
    foreach (CustomizationSortOption sort in payload.Sort)
    {
      switch (sort.Field)
      {
        case CustomizationSort.Name:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.Name) : ordered.ThenBy(x => x.Name));
          break;
        case CustomizationSort.UpdatedOn:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn));
          break;
      }
    }
    query = ordered ?? query;
    query = query.ApplyPaging(payload);

    CustomizationEntity[] customizations = await query.ToArrayAsync(cancellationToken);
    IEnumerable<CustomizationModel> items = await MapAsync(customizations, cancellationToken);

    return new SearchResults<CustomizationModel>(items, total);
  }

  private async Task<CustomizationModel> MapAsync(CustomizationEntity customization, CancellationToken cancellationToken)
  {
    return (await MapAsync([customization], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<CustomizationModel>> MapAsync(IEnumerable<CustomizationEntity> customizations, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = customizations.SelectMany(customization => customization.GetActorIds());
    IReadOnlyCollection<Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return customizations.Select(mapper.ToCustomization).ToArray();
  }
}
