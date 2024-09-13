using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Search;
using Microsoft.EntityFrameworkCore;
using SkillCraft.Application.Actors;
using SkillCraft.Application.Educations;
using SkillCraft.Contracts.Educations;
using SkillCraft.Domain.Educations;
using SkillCraft.Domain.Worlds;
using SkillCraft.EntityFrameworkCore.Entities;

namespace SkillCraft.EntityFrameworkCore.Queriers;

internal class EducationQuerier : IEducationQuerier
{
  private readonly IActorService _actorService;
  private readonly DbSet<EducationEntity> _educations;
  private readonly ISqlHelper _sqlHelper;

  public EducationQuerier(IActorService actorService, SkillCraftContext context, ISqlHelper sqlHelper)
  {
    _actorService = actorService;
    _educations = context.Educations;
    _sqlHelper = sqlHelper;
  }

  public async Task<EducationModel> ReadAsync(Education education, CancellationToken cancellationToken)
  {
    return await ReadAsync(education.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The education entity 'Id={education.Id.ToGuid()}' could not be found.");
  }
  public async Task<EducationModel?> ReadAsync(EducationId id, CancellationToken cancellationToken)
  {
    return await ReadAsync(id.ToGuid(), cancellationToken);
  }
  public async Task<EducationModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    EducationEntity? education = await _educations.AsNoTracking()
      .Include(x => x.World)
      .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    return education == null ? null : await MapAsync(education, cancellationToken);
  }

  public async Task<SearchResults<EducationModel>> SearchAsync(WorldId worldId, SearchEducationsPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _sqlHelper.QueryFrom(SkillCraftDb.Educations.Table).SelectAll(SkillCraftDb.Educations.Table)
      .Join(SkillCraftDb.Worlds.WorldId, SkillCraftDb.Educations.WorldId)
      .Where(SkillCraftDb.Worlds.Id, Operators.IsEqualTo(worldId.ToGuid()))
      .ApplyIdFilter(payload, SkillCraftDb.Educations.Id);
    _sqlHelper.ApplyTextSearch(builder, payload.Search, SkillCraftDb.Educations.Name);

    IQueryable<EducationEntity> query = _educations.FromQuery(builder).AsNoTracking()
      .Include(x => x.World);

    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<EducationEntity>? ordered = null;
    foreach (EducationSortOption sort in payload.Sort)
    {
      switch (sort.Field)
      {
        case EducationSort.Name:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.Name) : ordered.ThenBy(x => x.Name));
          break;
        case EducationSort.UpdatedOn:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn));
          break;
      }
    }
    query = ordered ?? query;
    query = query.ApplyPaging(payload);

    EducationEntity[] educations = await query.ToArrayAsync(cancellationToken);
    IEnumerable<EducationModel> items = await MapAsync(educations, cancellationToken);

    return new SearchResults<EducationModel>(items, total);
  }

  private async Task<EducationModel> MapAsync(EducationEntity education, CancellationToken cancellationToken)
  {
    return (await MapAsync([education], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<EducationModel>> MapAsync(IEnumerable<EducationEntity> educations, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = educations.SelectMany(education => education.GetActorIds());
    IReadOnlyCollection<Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return educations.Select(mapper.ToEducation).ToArray();
  }
}
