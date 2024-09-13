using Logitar.Portal.Contracts.Search;
using SkillCraft.Contracts.Educations;
using SkillCraft.Domain.Educations;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Educations;

public interface IEducationQuerier
{
  Task<EducationModel> ReadAsync(Education education, CancellationToken cancellationToken = default);
  Task<EducationModel?> ReadAsync(EducationId id, CancellationToken cancellationToken = default);
  Task<EducationModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);

  Task<SearchResults<EducationModel>> SearchAsync(WorldId worldId, SearchEducationsPayload payload, CancellationToken cancellationToken = default);
}
