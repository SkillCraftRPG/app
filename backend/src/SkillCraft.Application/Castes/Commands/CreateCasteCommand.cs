using FluentValidation;
using MediatR;
using SkillCraft.Application.Castes.Validators;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;
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
    new CreateCasteValidator().ValidateAndThrow(payload);

    await _permissionService.EnsureCanCreateAsync(command, EntityType.Caste, cancellationToken);

    UserId userId = command.GetUserId();
    Caste caste = new(command.GetWorldId(), new Name(payload.Name), userId)
    {
      Description = Description.TryCreate(payload.Description),
      Skill = payload.Skill,
      WealthRoll = Roll.TryCreate(payload.WealthRoll)
    };

    SetTraits(caste, payload);

    caste.Update(userId);
    await _sender.Send(new SaveCasteCommand(caste), cancellationToken);

    return await _casteQuerier.ReadAsync(caste, cancellationToken);
  }

  private static void SetTraits(Caste caste, CreateCastePayload payload)
  {
    foreach (TraitPayload traitPayload in payload.Traits)
    {
      Trait trait = new(new Name(traitPayload.Name), Description.TryCreate(traitPayload.Description));
      if (traitPayload.Id.HasValue)
      {
        caste.SetTrait(traitPayload.Id.Value, trait);
      }
      else
      {
        caste.AddTrait(trait);
      }
    }
  }
}
