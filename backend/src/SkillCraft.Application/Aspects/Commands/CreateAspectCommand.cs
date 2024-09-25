using FluentValidation;
using MediatR;
using SkillCraft.Application.Aspects.Validators;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Aspects;
using SkillCraft.Domain;
using SkillCraft.Domain.Aspects;

namespace SkillCraft.Application.Aspects.Commands;

public record CreateAspectCommand(CreateAspectPayload Payload) : Activity, IRequest<AspectModel>;

internal class CreateAspectCommandHandler : IRequestHandler<CreateAspectCommand, AspectModel>
{
  private readonly IAspectQuerier _aspectQuerier;
  private readonly IPermissionService _permissionService;
  private readonly ISender _sender;

  public CreateAspectCommandHandler(IAspectQuerier aspectQuerier, IPermissionService permissionService, ISender sender)
  {
    _aspectQuerier = aspectQuerier;
    _permissionService = permissionService;
    _sender = sender;
  }

  public async Task<AspectModel> Handle(CreateAspectCommand command, CancellationToken cancellationToken)
  {
    CreateAspectPayload payload = command.Payload;
    new CreateAspectValidator().ValidateAndThrow(payload);

    await _permissionService.EnsureCanCreateAsync(command, EntityType.Aspect, cancellationToken);

    UserId userId = command.GetUserId();
    Aspect aspect = new(command.GetWorldId(), new Name(payload.Name), userId)
    {
      Description = Description.TryCreate(payload.Description),
      Attributes = new AttributeSelection(payload.Attributes),
      Skills = new Skills(payload.Skills)
    };

    aspect.Update(userId);
    await _sender.Send(new SaveAspectCommand(aspect), cancellationToken);

    return await _aspectQuerier.ReadAsync(aspect, cancellationToken);
  }
}
