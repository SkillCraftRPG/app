using SkillCraft.Application.Storages;
using SkillCraft.Domain.Educations;

namespace SkillCraft.Application.Educations.Commands;

public abstract class EducationCommandHandler
{
  private readonly IEducationRepository _educationRepository;
  private readonly IStorageService _storageService;

  public EducationCommandHandler(IEducationRepository educationRepository, IStorageService storageService)
  {
    _educationRepository = educationRepository;
    _storageService = storageService;
  }

  protected async Task SaveAsync(Education education, CancellationToken cancellationToken)
  {
    EntityMetadata entity = education.GetMetadata();
    await _storageService.EnsureAvailableAsync(entity, cancellationToken);

    await _educationRepository.SaveAsync(education, cancellationToken);

    await _storageService.UpdateAsync(entity, cancellationToken);
  }
}
