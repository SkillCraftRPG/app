using MediatR;
using SkillCraft.Application.Storages;
using SkillCraft.Domain.Customizations;

namespace SkillCraft.Application.Customizations.Commands;

internal record SaveCustomizationCommand(Customization Customization) : IRequest;

internal class SaveCustomizationCommandHandler : IRequestHandler<SaveCustomizationCommand>
{
  private readonly ICustomizationRepository _CustomizationRepository;
  private readonly IStorageService _storageService;

  public SaveCustomizationCommandHandler(ICustomizationRepository CustomizationRepository, IStorageService storageService)
  {
    _CustomizationRepository = CustomizationRepository;
    _storageService = storageService;
  }

  public async Task Handle(SaveCustomizationCommand command, CancellationToken cancellationToken)
  {
    Customization customization = command.Customization;

    EntityMetadata entity = customization.GetMetadata();
    await _storageService.EnsureAvailableAsync(entity, cancellationToken);

    await _CustomizationRepository.SaveAsync(customization, cancellationToken);

    await _storageService.UpdateAsync(entity, cancellationToken);
  }
}
