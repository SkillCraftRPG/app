using MediatR;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts.Castes;
using SkillCraft.Domain;
using SkillCraft.Domain.Castes;

namespace SkillCraft.Application.Castes.Commands;

public record CreateCasteCommand(CreateCastePayload Payload) : Activity, IRequest<CasteModel>;

internal class CreateCasteCommandHandler : IRequestHandler<CreateCasteCommand, CasteModel>
{
  private readonly ICasteQuerier _casteQuerier;
  private readonly IPermissionService _permissionService;
  private readonly ISender _sender;

  public CreateCasteCommandHandler(ICasteQuerier casteQuerier, IPermissionService permissionService, ISender sender)
  {
    _casteQuerier = casteQuerier;
    _permissionService = permissionService;
    _sender = sender;
  }

  public async Task<CasteModel> Handle(CreateCasteCommand command, CancellationToken cancellationToken)
  {
    CreateCastePayload payload = command.Payload;
    //new CreateCasteValidator().ValidateAndThrow(payload); // TODO(fpion): implement

    await _permissionService.EnsureCanCreateAsync(command, EntityType.Caste, cancellationToken);

    UserId userId = command.GetUserId();
    Caste caste = new(command.GetWorldId(), new Name(payload.Name), userId)
    {
      Description = Description.TryCreate(payload.Description),
      Skill = payload.Skill,
      WealthRoll = Roll.TryCreate(payload.WealthRoll)
    };

    // TODO(fpion): Traits

    caste.Update(userId);
    await _sender.Send(new SaveCasteCommand(caste), cancellationToken);

    return await _casteQuerier.ReadAsync(caste, cancellationToken);
  }
}
