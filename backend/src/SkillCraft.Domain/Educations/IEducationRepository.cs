namespace SkillCraft.Domain.Educations;

public interface IEducationRepository
{
  Task<IReadOnlyCollection<Education>> LoadAsync(CancellationToken cancellationToken = default);

  Task<Education?> LoadAsync(EducationId id, CancellationToken cancellationToken = default);
  Task<Education?> LoadAsync(EducationId id, long? version, CancellationToken cancellationToken = default);

  Task SaveAsync(Education education, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<Education> educations, CancellationToken cancellationToken = default);
}
