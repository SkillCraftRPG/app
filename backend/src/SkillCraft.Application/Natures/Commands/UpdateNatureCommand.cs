using FluentValidation;
using MediatR;
using SkillCraft.Application.Customizations;
using SkillCraft.Application.Natures.Validators;
using SkillCraft.Application.Permissions;
using SkillCraft.Application.Storages;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Natures;
using SkillCraft.Domain;
using SkillCraft.Domain.Natures;

namespace SkillCraft.Application.Natures.Commands;

/// <exception cref="CustomizationIsNotGiftException"></exception>
/// <exception cref="CustomizationNotFoundException"></exception>
/// <exception cref="NotEnoughAvailableStorageException"></exception>
/// <exception cref="PermissionDeniedException"></exception>
/// <exception cref="ValidationException"></exception>
public record UpdateNatureCommand(Guid Id, UpdateNaturePayload Payload) : Activity, IRequest<NatureModel?>;

internal class UpdateNatureCommandHandler : IRequestHandler<UpdateNatureCommand, NatureModel?>
{
  private readonly INatureQuerier _natureQuerier;
  private readonly INatureRepository _natureRepository;
  private readonly IPermissionService _permissionService;
  private readonly ISender _sender;

  public UpdateNatureCommandHandler(
    INatureQuerier natureQuerier,
    INatureRepository natureRepository,
    IPermissionService permissionService,
    ISender sender)
  {
    _natureQuerier = natureQuerier;
    _natureRepository = natureRepository;
    _permissionService = permissionService;
    _sender = sender;
  }

  public async Task<NatureModel?> Handle(UpdateNatureCommand command, CancellationToken cancellationToken)
  {
    UpdateNaturePayload payload = command.Payload;
    new UpdateNatureValidator().ValidateAndThrow(payload);

    NatureId id = new(command.GetWorldId(), command.Id);
    Nature? nature = await _natureRepository.LoadAsync(id, cancellationToken);
    if (nature == null)
    {
      return null;
    }

    await _permissionService.EnsureCanPreviewAsync(command, EntityType.Customization, cancellationToken);
    await _permissionService.EnsureCanUpdateAsync(command, nature.GetMetadata(), cancellationToken);

    if (!string.IsNullOrWhiteSpace(payload.Name))
    {
      nature.Name = new Name(payload.Name);
    }
    if (payload.Description != null)
    {
      nature.Description = Description.TryCreate(payload.Description.Value);
    }

    if (payload.Attribute != null)
    {
      nature.Attribute = payload.Attribute.Value;
    }
    if (payload.GiftId != null)
    {
      await _sender.Send(new SetGiftCommand(nature, payload.GiftId.Value), cancellationToken);
    }

    nature.Update(command.GetUserId());

    await _sender.Send(new SaveNatureCommand(nature), cancellationToken);

    return await _natureQuerier.ReadAsync(nature, cancellationToken);
  }
}
