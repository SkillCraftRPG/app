using FluentValidation;
using MediatR;
using SkillCraft.Application.Permissions;
using SkillCraft.Application.Talents.Validators;
using SkillCraft.Contracts.Talents;
using SkillCraft.Domain;
using SkillCraft.Domain.Talents;

namespace SkillCraft.Application.Talents.Commands;

public record UpdateTalentCommand(Guid Id, UpdateTalentPayload Payload) : Activity, IRequest<TalentModel?>;

internal class UpdateTalentCommandHandler : IRequestHandler<UpdateTalentCommand, TalentModel?>
{
  private readonly IPermissionService _permissionService;
  private readonly ISender _sender;
  private readonly ITalentQuerier _talentQuerier;
  private readonly ITalentRepository _talentRepository;

  public UpdateTalentCommandHandler(
    IPermissionService permissionService,
    ISender sender,
    ITalentQuerier talentQuerier,
    ITalentRepository talentRepository)
  {
    _permissionService = permissionService;
    _sender = sender;
    _talentQuerier = talentQuerier;
    _talentRepository = talentRepository;
  }

  public async Task<TalentModel?> Handle(UpdateTalentCommand command, CancellationToken cancellationToken)
  {
    UpdateTalentPayload payload = command.Payload;
    new UpdateTalentValidator().ValidateAndThrow(payload);

    TalentId id = new(command.GetWorldId(), command.Id);
    Talent? talent = await _talentRepository.LoadAsync(id, cancellationToken);
    if (talent == null)
    {
      return null;
    }

    await _permissionService.EnsureCanUpdateAsync(command, talent.GetMetadata(), cancellationToken);

    if (!string.IsNullOrWhiteSpace(payload.Name))
    {
      talent.Name = new Name(payload.Name);
    }
    if (payload.Description != null)
    {
      talent.Description = Description.TryCreate(payload.Description.Value);
    }

    if (payload.AllowMultiplePurchases.HasValue)
    {
      talent.AllowMultiplePurchases = payload.AllowMultiplePurchases.Value;
    }
    if (payload.RequiredTalentId != null)
    {
      await _sender.Send(new SetRequiredTalentCommand(command, talent, payload.RequiredTalentId.Value), cancellationToken);
    }

    talent.Update(command.GetUserId());
    await _sender.Send(new SaveTalentCommand(talent), cancellationToken);

    return await _talentQuerier.ReadAsync(talent, cancellationToken);
  }
}
