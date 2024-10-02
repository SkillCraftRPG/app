using FluentValidation;
using MediatR;
using SkillCraft.Application.Customizations.Validators;
using SkillCraft.Application.Permissions;
using SkillCraft.Application.Storages;
using SkillCraft.Contracts.Customizations;
using SkillCraft.Domain;
using SkillCraft.Domain.Customizations;

namespace SkillCraft.Application.Customizations.Commands;

public record UpdateCustomizationCommand(Guid Id, UpdateCustomizationPayload Payload) : Activity, IRequest<CustomizationModel?>;

internal class UpdateCustomizationCommandHandler : CustomizationCommandHandler, IRequestHandler<UpdateCustomizationCommand, CustomizationModel?>
{
  private readonly ICustomizationQuerier _customizationQuerier;
  private readonly ICustomizationRepository _customizationRepository;
  private readonly IPermissionService _permissionService;

  public UpdateCustomizationCommandHandler(
    ICustomizationQuerier customizationQuerier,
    ICustomizationRepository customizationRepository,
    IPermissionService permissionService,
    IStorageService storageService) : base(customizationRepository, storageService)
  {
    _customizationQuerier = customizationQuerier;
    _customizationRepository = customizationRepository;
    _permissionService = permissionService;
  }

  public async Task<CustomizationModel?> Handle(UpdateCustomizationCommand command, CancellationToken cancellationToken)
  {
    UpdateCustomizationPayload payload = command.Payload;
    new UpdateCustomizationValidator().ValidateAndThrow(payload);

    CustomizationId id = new(command.GetWorldId(), command.Id);
    Customization? customization = await _customizationRepository.LoadAsync(id, cancellationToken);
    if (customization == null)
    {
      return null;
    }

    await _permissionService.EnsureCanUpdateAsync(command, customization.GetMetadata(), cancellationToken);

    if (!string.IsNullOrWhiteSpace(payload.Name))
    {
      customization.Name = new Name(payload.Name);
    }
    if (payload.Description != null)
    {
      customization.Description = Description.TryCreate(payload.Description.Value);
    }

    customization.Update(command.GetUserId());

    await SaveAsync(customization, cancellationToken);

    return await _customizationQuerier.ReadAsync(customization, cancellationToken);
  }
}
