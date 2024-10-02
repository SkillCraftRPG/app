using FluentValidation;
using MediatR;
using SkillCraft.Application.Customizations.Validators;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts.Customizations;
using SkillCraft.Domain;
using SkillCraft.Domain.Customizations;

namespace SkillCraft.Application.Customizations.Commands;

public record ReplaceCustomizationCommand(Guid Id, ReplaceCustomizationPayload Payload, long? Version) : Activity, IRequest<CustomizationModel?>;

internal class ReplaceCustomizationCommandHandler : IRequestHandler<ReplaceCustomizationCommand, CustomizationModel?>
{
  private readonly ICustomizationQuerier _customizationQuerier;
  private readonly ICustomizationRepository _customizationRepository;
  private readonly IPermissionService _permissionService;
  private readonly ISender _sender;

  public ReplaceCustomizationCommandHandler(
    ICustomizationQuerier customizationQuerier,
    ICustomizationRepository customizationRepository,
    IPermissionService permissionService,
    ISender sender)
  {
    _customizationQuerier = customizationQuerier;
    _customizationRepository = customizationRepository;
    _permissionService = permissionService;
    _sender = sender;
  }

  public async Task<CustomizationModel?> Handle(ReplaceCustomizationCommand command, CancellationToken cancellationToken)
  {
    ReplaceCustomizationPayload payload = command.Payload;
    new ReplaceCustomizationValidator().ValidateAndThrow(payload);

    CustomizationId id = new(command.GetWorldId(), command.Id);
    Customization? customization = await _customizationRepository.LoadAsync(id, cancellationToken);
    if (customization == null)
    {
      return null;
    }

    await _permissionService.EnsureCanUpdateAsync(command, customization.GetMetadata(), cancellationToken);

    Customization? reference = null;
    if (command.Version.HasValue)
    {
      reference = await _customizationRepository.LoadAsync(id, command.Version.Value, cancellationToken);
    }
    reference ??= customization;

    Name name = new(payload.Name);
    if (name != reference.Name)
    {
      customization.Name = name;
    }
    Description? description = Description.TryCreate(payload.Description);
    if (description != reference.Description)
    {
      customization.Description = description;
    }

    customization.Update(command.GetUserId());
    await _sender.Send(new SaveCustomizationCommand(customization), cancellationToken);

    return await _customizationQuerier.ReadAsync(customization, cancellationToken);
  }
}
