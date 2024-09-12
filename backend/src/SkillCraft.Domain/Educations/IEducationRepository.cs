namespace SkillCraft.Domain.Educations;

public interface IEducationRepository
{
  Task SaveAsync(Education education, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<Education> educations, CancellationToken cancellationToken = default);
}
