using SkillCraft.Contracts.Educations;
using SkillCraft.Domain.Educations;

namespace SkillCraft.Application.Educations;

public interface IEducationQuerier
{
  Task<EducationModel> ReadAsync(Education education, CancellationToken cancellationToken = default);
  Task<EducationModel?> ReadAsync(EducationId id, CancellationToken cancellationToken = default);
  Task<EducationModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
}
