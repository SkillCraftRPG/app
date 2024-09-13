using MediatR;
using SkillCraft.Application.Storages;
using SkillCraft.Domain.Castes;

namespace SkillCraft.Application.Castes.Commands;

internal record SaveCasteCommand(Caste Caste) : IRequest;

internal class SaveCasteCommandHandler : IRequestHandler<SaveCasteCommand>
{
  private readonly ICasteRepository _casteRepository;
  private readonly IStorageService _storageService;

  public SaveCasteCommandHandler(ICasteRepository casteRepository, IStorageService storageService)
  {
    _casteRepository = casteRepository;
    _storageService = storageService;
  }

  public async Task Handle(SaveCasteCommand command, CancellationToken cancellationToken)
  {
    Caste caste = command.Caste;

    EntityMetadata entity = caste.GetMetadata();
    await _storageService.EnsureAvailableAsync(entity, cancellationToken);

    await _casteRepository.SaveAsync(caste, cancellationToken);

    await _storageService.UpdateAsync(entity, cancellationToken);
  }
}
