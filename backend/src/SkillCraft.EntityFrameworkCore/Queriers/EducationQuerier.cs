using Logitar.EventSourcing;
using Logitar.Portal.Contracts.Actors;
using Microsoft.EntityFrameworkCore;
using SkillCraft.Application.Actors;
using SkillCraft.Application.Educations;
using SkillCraft.Contracts.Educations;
using SkillCraft.Domain.Educations;
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
      .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    return education == null ? null : await MapAsync(education, cancellationToken);
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
