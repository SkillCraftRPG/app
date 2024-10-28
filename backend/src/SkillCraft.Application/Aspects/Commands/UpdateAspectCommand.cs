using FluentValidation;
using MediatR;
using SkillCraft.Application.Aspects.Validators;
using SkillCraft.Application.Permissions;
using SkillCraft.Application.Storages;
using SkillCraft.Contracts.Aspects;
using SkillCraft.Domain;
using SkillCraft.Domain.Aspects;

namespace SkillCraft.Application.Aspects.Commands;

/// <exception cref="NotEnoughAvailableStorageException"></exception>
/// <exception cref="PermissionDeniedException"></exception>
/// <exception cref="ValidationException"></exception>
public record UpdateAspectCommand(Guid Id, UpdateAspectPayload Payload) : Activity, IRequest<AspectModel?>;

internal class UpdateAspectCommandHandler : IRequestHandler<UpdateAspectCommand, AspectModel?>
{
  private readonly IAspectQuerier _aspectQuerier;
  private readonly IAspectRepository _aspectRepository;
  private readonly IPermissionService _permissionService;
  private readonly ISender _sender;

  public UpdateAspectCommandHandler(
    IAspectQuerier aspectQuerier,
    IAspectRepository aspectRepository,
    IPermissionService permissionService,
    ISender sender)
  {
    _aspectQuerier = aspectQuerier;
    _aspectRepository = aspectRepository;
    _permissionService = permissionService;
    _sender = sender;
  }

  public async Task<AspectModel?> Handle(UpdateAspectCommand command, CancellationToken cancellationToken)
  {
    UpdateAspectPayload payload = command.Payload;
    new UpdateAspectValidator().ValidateAndThrow(payload);

    AspectId id = new(command.GetWorldId(), command.Id);
    Aspect? aspect = await _aspectRepository.LoadAsync(id, cancellationToken);
    if (aspect == null)
    {
      return null;
    }

    await _permissionService.EnsureCanUpdateAsync(command, aspect.GetMetadata(), cancellationToken);

    if (!string.IsNullOrWhiteSpace(payload.Name))
    {
      aspect.Name = new Name(payload.Name);
    }
    if (payload.Description != null)
    {
      aspect.Description = Description.TryCreate(payload.Description.Value);
    }

    if (payload.Attributes != null)
    {
      aspect.Attributes = new AttributeSelection(payload.Attributes);
    }
    if (payload.Skills != null)
    {
      aspect.Skills = new Skills(payload.Skills);
    }

    aspect.Update(command.GetUserId());

    await _sender.Send(new SaveAspectCommand(aspect), cancellationToken);

    return await _aspectQuerier.ReadAsync(aspect, cancellationToken);
  }
}
