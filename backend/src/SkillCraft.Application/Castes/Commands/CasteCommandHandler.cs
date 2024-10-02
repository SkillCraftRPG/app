using SkillCraft.Application.Storages;
using SkillCraft.Domain.Castes;

namespace SkillCraft.Application.Castes.Commands;

public abstract class CasteCommandHandler
{
  private readonly ICasteRepository _casteRepository;
  private readonly IStorageService _storageService;

  public CasteCommandHandler(ICasteRepository casteRepository, IStorageService storageService)
  {
    _casteRepository = casteRepository;
    _storageService = storageService;
  }

  protected async Task SaveAsync(Caste caste, CancellationToken cancellationToken)
  {
    EntityMetadata entity = caste.GetMetadata();
    await _storageService.EnsureAvailableAsync(entity, cancellationToken);

    await _casteRepository.SaveAsync(caste, cancellationToken);

    await _storageService.UpdateAsync(entity, cancellationToken);
  }
}
