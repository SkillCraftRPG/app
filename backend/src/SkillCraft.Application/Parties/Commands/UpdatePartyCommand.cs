using FluentValidation;
using MediatR;
using SkillCraft.Application.Parties.Validators;
using SkillCraft.Application.Permissions;
using SkillCraft.Application.Storages;
using SkillCraft.Contracts.Parties;
using SkillCraft.Domain;
using SkillCraft.Domain.Parties;

namespace SkillCraft.Application.Parties.Commands;

public record UpdatePartyCommand(Guid Id, UpdatePartyPayload Payload) : Activity, IRequest<PartyModel?>;

internal class UpdatePartyCommandHandler : PartyCommandHandler, IRequestHandler<UpdatePartyCommand, PartyModel?>
{
  private readonly IPartyQuerier _partyQuerier;
  private readonly IPartyRepository _partyRepository;
  private readonly IPermissionService _permissionService;

  public UpdatePartyCommandHandler(
    IPartyQuerier partyQuerier,
    IPartyRepository partyRepository,
    IPermissionService permissionService,
    IStorageService storageService) : base(partyRepository, storageService)
  {
    _partyQuerier = partyQuerier;
    _partyRepository = partyRepository;
    _permissionService = permissionService;
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

    await SaveAsync(party, cancellationToken);

    return await _partyQuerier.ReadAsync(party, cancellationToken);
  }
}
