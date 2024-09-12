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

  public async Task SaveAsync(Education education, CancellationToken cancellationToken)
  {
    await base.SaveAsync(education, cancellationToken);
  }
  public async Task SaveAsync(IEnumerable<Education> educations, CancellationToken cancellationToken)
  {
    await base.SaveAsync(educations, cancellationToken);
  }
}
