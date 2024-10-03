using MediatR;
using SkillCraft.Application.Storages;
using SkillCraft.Domain.Customizations;

namespace SkillCraft.Application.Customizations.Commands;

internal record SaveCustomizationCommand(Customization Customization) : IRequest;

internal class SaveCustomizationCommandHandler : IRequestHandler<SaveCustomizationCommand>
{
  private readonly ICustomizationRepository _customizationRepository;
  private readonly IStorageService _storageService;

  public SaveCustomizationCommandHandler(ICustomizationRepository customizationRepository, IStorageService storageService)
  {
    _customizationRepository = customizationRepository;
    _storageService = storageService;
  }

  public async Task Handle(SaveCustomizationCommand command, CancellationToken cancellationToken)
  {
    Customization customization = command.Customization;

    EntityMetadata entity = customization.GetMetadata();
    await _storageService.EnsureAvailableAsync(entity, cancellationToken);

    await _customizationRepository.SaveAsync(customization, cancellationToken);

    await _storageService.UpdateAsync(entity, cancellationToken);
  }
}
