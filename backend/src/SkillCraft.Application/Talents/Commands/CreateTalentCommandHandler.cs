using FluentValidation;
using MediatR;
using SkillCraft.Application.Permissions;
using SkillCraft.Application.Talents.Validators;
using SkillCraft.Contracts.Talents;
using SkillCraft.Domain;
using SkillCraft.Domain.Talents;

namespace SkillCraft.Application.Talents.Commands;

public record CreateTalentCommand(CreateTalentPayload Payload) : Activity, IRequest<TalentModel>;

internal class CreateTalentCommandHandler : IRequestHandler<CreateTalentCommand, TalentModel>
{
  private readonly IPermissionService _permissionService;
  private readonly ISender _sender;
  private readonly ITalentQuerier _talentQuerier;

  public CreateTalentCommandHandler(IPermissionService permissionService, ISender sender, ITalentQuerier talentQuerier)
  {
    _permissionService = permissionService;
    _sender = sender;
    _talentQuerier = talentQuerier;
  }

  public async Task<TalentModel> Handle(CreateTalentCommand command, CancellationToken cancellationToken)
  {
    CreateTalentPayload payload = command.Payload;
    new CreateTalentValidator().ValidateAndThrow(payload);

    await _permissionService.EnsureCanCreateAsync(command, EntityType.Talent, cancellationToken);

    UserId userId = command.GetUserId();
    Talent talent = new(command.GetWorldId(), payload.Tier, new Name(payload.Name), userId)
    {
      Description = Description.TryCreate(payload.Description),
      AllowMultiplePurchases = payload.AllowMultiplePurchases
    };
    if (payload.RequiredTalentId.HasValue)
    {
      await _sender.Send(new SetRequiredTalentCommand(command, talent, payload.RequiredTalentId.Value), cancellationToken);
    }

    talent.Update(userId);
    await _sender.Send(new SaveTalentCommand(talent), cancellationToken);

    return await _talentQuerier.ReadAsync(talent, cancellationToken);
  }
}
