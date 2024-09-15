using FluentValidation;
using MediatR;
using SkillCraft.Application.Customizations.Validators;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts.Customizations;
using SkillCraft.Domain;
using SkillCraft.Domain.Customizations;

namespace SkillCraft.Application.Customizations.Commands;

public record CreateCustomizationCommand(CreateCustomizationPayload Payload) : Activity, IRequest<CustomizationModel>;

internal class CreateCustomizationCommandHandler : IRequestHandler<CreateCustomizationCommand, CustomizationModel>
{
  private readonly ICustomizationQuerier _customizationQuerier;
  private readonly IPermissionService _permissionService;
  private readonly ISender _sender;

  public CreateCustomizationCommandHandler(ICustomizationQuerier customizationQuerier, IPermissionService permissionService, ISender sender)
  {
    _customizationQuerier = customizationQuerier;
    _permissionService = permissionService;
    _sender = sender;
  }

  public async Task<CustomizationModel> Handle(CreateCustomizationCommand command, CancellationToken cancellationToken)
  {
    CreateCustomizationPayload payload = command.Payload;
    new CreateCustomizationValidator().ValidateAndThrow(payload);

    await _permissionService.EnsureCanCreateAsync(command, EntityType.Customization, cancellationToken);

    UserId userId = command.GetUserId();
    Customization customization = new(command.GetWorldId(), payload.Type, new Name(payload.Name), userId)
    {
      Description = Description.TryCreate(payload.Description)
    };

    customization.Update(userId);
    await _sender.Send(new SaveCustomizationCommand(customization), cancellationToken);

    return await _customizationQuerier.ReadAsync(customization, cancellationToken);
  }
}
