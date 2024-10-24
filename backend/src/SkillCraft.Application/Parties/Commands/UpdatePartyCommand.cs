using FluentValidation;
using MediatR;
using SkillCraft.Application.Parties.Validators;
using SkillCraft.Application.Permissions;
using SkillCraft.Application.Storages;
using SkillCraft.Contracts.Parties;
using SkillCraft.Domain;
using SkillCraft.Domain.Parties;

namespace SkillCraft.Application.Parties.Commands;

/// <exception cref="NotEnoughAvailableStorageException"></exception>
/// <exception cref="PermissionDeniedException"></exception>
/// <exception cref="ValidationException"></exception>
public record UpdatePartyCommand(Guid Id, UpdatePartyPayload Payload) : Activity, IRequest<PartyModel?>;

internal class UpdatePartyCommandHandler : IRequestHandler<UpdatePartyCommand, PartyModel?>
{
  private readonly IPartyQuerier _partyQuerier;
  private readonly IPartyRepository _partyRepository;
  private readonly IPermissionService _permissionService;
  private readonly ISender _sender;

  public UpdatePartyCommandHandler(
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

  public async Task<PartyModel?> Handle(UpdatePartyCommand command, CancellationToken cancellationToken)
  {
    UpdatePartyPayload payload = command.Payload;
    new UpdatePartyValidator().ValidateAndThrow(payload);

    PartyId id = new(command.GetWorldId(), command.Id);
    Party? party = await _partyRepository.LoadAsync(id, cancellationToken);
    if (party == null)
    {
      return null;
    }

    await _permissionService.EnsureCanUpdateAsync(command, party.GetMetadata(), cancellationToken);

    if (!string.IsNullOrWhiteSpace(payload.Name))
    {
      party.Name = new Name(payload.Name);
    }
    if (payload.Description != null)
    {
      party.Description = Description.TryCreate(payload.Description.Value);
    }

    party.Update(command.GetUserId());

    await _sender.Send(new SavePartyCommand(party), cancellationToken);

    return await _partyQuerier.ReadAsync(party, cancellationToken);
  }
}
