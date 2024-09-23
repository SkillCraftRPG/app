using FluentValidation;
using MediatR;
using SkillCraft.Application.Permissions;
using SkillCraft.Application.Talents.Validators;
using SkillCraft.Contracts.Talents;
using SkillCraft.Domain;
using SkillCraft.Domain.Talents;

namespace SkillCraft.Application.Talents.Commands;

public record ReplaceTalentCommand(Guid Id, ReplaceTalentPayload Payload, long? Version) : Activity, IRequest<TalentModel?>;

internal class ReplaceTalentCommandHandler : IRequestHandler<ReplaceTalentCommand, TalentModel?>
{
  private readonly IPermissionService _permissionService;
  private readonly ISender _sender;
  private readonly ITalentQuerier _talentQuerier;
  private readonly ITalentRepository _talentRepository;

  public ReplaceTalentCommandHandler(
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

  public async Task<TalentModel?> Handle(ReplaceTalentCommand command, CancellationToken cancellationToken)
  {
    ReplaceTalentPayload payload = command.Payload;
    new ReplaceTalentValidator().ValidateAndThrow(payload);

    TalentId id = new(command.Id);
    Talent? talent = await _talentRepository.LoadAsync(id, cancellationToken);
    if (talent == null)
    {
      return null;
    }

    await _permissionService.EnsureCanUpdateAsync(command, talent.GetMetadata(), cancellationToken);

    Talent? reference = null;
    if (command.Version.HasValue)
    {
      reference = await _talentRepository.LoadAsync(id, command.Version.Value, cancellationToken);
    }
    reference ??= talent;

    Name name = new(payload.Name);
    if (name != reference.Name)
    {
      talent.Name = name;
    }
    Description? description = Description.TryCreate(payload.Description);
    if (description != reference.Description)
    {
      talent.Description = description;
    }

    if (payload.AllowMultiplePurchases != reference.AllowMultiplePurchases)
    {
      talent.AllowMultiplePurchases = payload.AllowMultiplePurchases;
    }
    TalentId? requiredTalentId = payload.RequiredTalentId.HasValue ? new(payload.RequiredTalentId.Value) : null;
    if (requiredTalentId != reference.RequiredTalentId)
    {
      await _sender.Send(new SetRequiredTalentCommand(command, talent, payload.RequiredTalentId), cancellationToken);
    }
    if (payload.Skill != reference.Skill)
    {
      talent.Skill = payload.Skill;
    }

    talent.Update(command.GetUserId());
    await _sender.Send(new SaveTalentCommand(talent), cancellationToken);

    return await _talentQuerier.ReadAsync(talent, cancellationToken);
  }
}
