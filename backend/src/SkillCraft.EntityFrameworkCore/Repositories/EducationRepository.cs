using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.EventSourcing.Infrastructure;
using SkillCraft.Domain.Educations;

namespace SkillCraft.EntityFrameworkCore.Repositories;

internal class EducationRepository : Logitar.EventSourcing.EntityFrameworkCore.Relational.AggregateRepository, IEducationRepository
{
  public EducationRepository(IEventBus eventBus, EventContext eventContext, IEventSerializer eventSerializer)
    : base(eventBus, eventContext, eventSerializer)
  {
  }

  public async Task<IReadOnlyCollection<Education>> LoadAsync(CancellationToken cancellationToken)
  {
    return (await LoadAsync<Education>(cancellationToken)).ToArray();
  }

  public async Task<Education?> LoadAsync(EducationId id, CancellationToken cancellationToken)
  {
    return await LoadAsync(id, version: null, cancellationToken);
  }
  public async Task<Education?> LoadAsync(EducationId id, long? version, CancellationToken cancellationToken)
  {
    return await LoadAsync<Education>(id.AggregateId, version, cancellationToken);
  }

  public async Task SaveAsync(Education education, CancellationToken cancellationToken)
  {
    await base.SaveAsync(education, cancellationToken);
  }
  public async Task SaveAsync(IEnumerable<Education> educations, CancellationToken cancellationToken)
  {
    await base.SaveAsync(educations, cancellationToken);
  }
}
