using SkillCraft.Application.Storages;
using SkillCraft.Domain.Customizations;

namespace SkillCraft.Application.Customizations.Commands;

internal abstract class CustomizationCommandHandler
{
  private readonly ICustomizationRepository _customizationRepository;
  private readonly IStorageService _storageService;

  public CustomizationCommandHandler(ICustomizationRepository customizationRepository, IStorageService storageService)
  {
    _customizationRepository = customizationRepository;
    _storageService = storageService;
  }

  protected async Task SaveAsync(Customization customization, CancellationToken cancellationToken)
  {
    EntityMetadata entity = customization.GetMetadata();
    await _storageService.EnsureAvailableAsync(entity, cancellationToken);

    await _customizationRepository.SaveAsync(customization, cancellationToken);

    await _storageService.UpdateAsync(entity, cancellationToken);
  }
}
