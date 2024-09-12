using MediatR;
using SkillCraft.Application.Storages;
using SkillCraft.Domain.Educations;

namespace SkillCraft.Application.Educations.Commands;

internal record SaveEducationCommand(Education Education) : IRequest;

internal class SaveEducationCommandHandler : IRequestHandler<SaveEducationCommand>
{
  private readonly IEducationRepository _educationRepository;
  private readonly IStorageService _storageService;

  public SaveEducationCommandHandler(IEducationRepository educationRepository, IStorageService storageService)
  {
    _educationRepository = educationRepository;
    _storageService = storageService;
  }

  public async Task Handle(SaveEducationCommand command, CancellationToken cancellationToken)
  {
    Education education = command.Education;

    EntityMetadata entity = EntityMetadata.From(education);
    await _storageService.EnsureAvailableAsync(entity, cancellationToken);

    await _educationRepository.SaveAsync(education, cancellationToken);

    await _storageService.UpdateAsync(entity, cancellationToken);
  }
}
