using FluentValidation;
using MediatR;
using SkillCraft.Application.Aspects.Validators;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts.Aspects;
using SkillCraft.Domain;
using SkillCraft.Domain.Aspects;

namespace SkillCraft.Application.Aspects.Commands;

public record ReplaceAspectCommand(Guid Id, ReplaceAspectPayload Payload, long? Version) : Activity, IRequest<AspectModel?>;

internal class ReplaceAspectCommandHandler : IRequestHandler<ReplaceAspectCommand, AspectModel?>
{
  private readonly IAspectQuerier _aspectQuerier;
  private readonly IAspectRepository _aspectRepository;
  private readonly IPermissionService _permissionService;
  private readonly ISender _sender;

  public ReplaceAspectCommandHandler(
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

  public async Task<AspectModel?> Handle(ReplaceAspectCommand command, CancellationToken cancellationToken)
  {
    ReplaceAspectPayload payload = command.Payload;
    new ReplaceAspectValidator().ValidateAndThrow(payload);

    AspectId id = new(command.GetWorldId(), command.Id);
    Aspect? aspect = await _aspectRepository.LoadAsync(id, cancellationToken);
    if (aspect == null)
    {
      return null;
    }

    await _permissionService.EnsureCanUpdateAsync(command, aspect.GetMetadata(), cancellationToken);

    Aspect? reference = null;
    if (command.Version.HasValue)
    {
      reference = await _aspectRepository.LoadAsync(id, command.Version.Value, cancellationToken);
    }
    reference ??= aspect;

    Name name = new(payload.Name);
    if (name != reference.Name)
    {
      aspect.Name = name;
    }
    Description? description = Description.TryCreate(payload.Description);
    if (description != reference.Description)
    {
      aspect.Description = description;
    }

    AttributeSelection attributes = new(payload.Attributes);
    if (attributes != reference.Attributes)
    {
      aspect.Attributes = attributes;
    }
    Skills skills = new(payload.Skills);
    if (skills != reference.Skills)
    {
      aspect.Skills = skills;
    }

    aspect.Update(command.GetUserId());
    await _sender.Send(new SaveAspectCommand(aspect), cancellationToken);

    return await _aspectQuerier.ReadAsync(aspect, cancellationToken);
  }
}
