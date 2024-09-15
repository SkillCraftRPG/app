using MediatR;
using SkillCraft.Application.Storages;
using SkillCraft.Domain.Aspects;

namespace SkillCraft.Application.Aspects.Commands;

internal record SaveAspectCommand(Aspect Aspect) : IRequest;

internal class SaveAspectCommandHandler : IRequestHandler<SaveAspectCommand>
{
  private readonly IAspectRepository _aspectRepository;
  private readonly IStorageService _storageService;

  public SaveAspectCommandHandler(IAspectRepository aspectRepository, IStorageService storageService)
  {
    _aspectRepository = aspectRepository;
    _storageService = storageService;
  }

  public async Task Handle(SaveAspectCommand command, CancellationToken cancellationToken)
  {
    Aspect aspect = command.Aspect;

    EntityMetadata entity = aspect.GetMetadata();
    await _storageService.EnsureAvailableAsync(entity, cancellationToken);

    await _aspectRepository.SaveAsync(aspect, cancellationToken);

    await _storageService.UpdateAsync(entity, cancellationToken);
  }
}
