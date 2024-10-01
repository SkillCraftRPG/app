using FluentValidation;
using MediatR;
using SkillCraft.Application.Parties.Validators;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts.Parties;
using SkillCraft.Domain;
using SkillCraft.Domain.Parties;

namespace SkillCraft.Application.Parties.Commands;

public record ReplacePartyCommand(Guid Id, ReplacePartyPayload Payload, long? Version) : Activity, IRequest<PartyModel?>;

internal class ReplacePartyCommandHandler : IRequestHandler<ReplacePartyCommand, PartyModel?>
{
  private readonly IPartyQuerier _partyQuerier;
  private readonly IPartyRepository _partyRepository;
  private readonly IPermissionService _permissionService;
  private readonly ISender _sender;

  public ReplacePartyCommandHandler(
    IPartyQuerier partyQuerier,
    IPartyRepository partyRepository,
    IPermissionService permissionService,
    ISender sender)
  {
    _partyQuerier = partyQuerier;
    _partyRepository = partyRepository;
    _permissionService = permissionService;
    _sender = sender;
  }

  public async Task<PartyModel?> Handle(ReplacePartyCommand command, CancellationToken cancellationToken)
  {
    ReplacePartyPayload payload = command.Payload;
    new ReplacePartyValidator().ValidateAndThrow(payload);

    PartyId id = new(command.GetWorldId(), command.Id);
    Party? party = await _partyRepository.LoadAsync(id, cancellationToken);
    if (party == null)
    {
      return null;
    }

    await _permissionService.EnsureCanUpdateAsync(command, party.GetMetadata(), cancellationToken);

    Party? reference = null;
    if (command.Version.HasValue)
    {
      reference = await _partyRepository.LoadAsync(id, command.Version.Value, cancellationToken);
    }
    reference ??= party;

    Name name = new(payload.Name);
    if (name != reference.Name)
    {
      party.Name = name;
    }
    Description? description = Description.TryCreate(payload.Description);
    if (description != reference.Description)
    {
      party.Description = description;
    }

    party.Update(command.GetUserId());
    await _sender.Send(new SavePartyCommand(party), cancellationToken);

    return await _partyQuerier.ReadAsync(party, cancellationToken);
  }
}
