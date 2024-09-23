using FluentValidation;
using MediatR;
using SkillCraft.Application.Parties.Validators;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts.Parties;
using SkillCraft.Domain;
using SkillCraft.Domain.Parties;

namespace SkillCraft.Application.Parties.Commands;

public record CreatePartyCommand(CreatePartyPayload Payload) : Activity, IRequest<PartyModel>;

internal class CreatePartyCommandHandler : IRequestHandler<CreatePartyCommand, PartyModel>
{
  private readonly IPartyQuerier _partyQuerier;
  private readonly IPermissionService _permissionService;
  private readonly ISender _sender;

  public CreatePartyCommandHandler(IPartyQuerier partyQuerier, IPermissionService permissionService, ISender sender)
  {
    _partyQuerier = partyQuerier;
    _permissionService = permissionService;
    _sender = sender;
  }

  public async Task<PartyModel> Handle(CreatePartyCommand command, CancellationToken cancellationToken)
  {
    CreatePartyPayload payload = command.Payload;
    new CreatePartyValidator().ValidateAndThrow(payload);

    await _permissionService.EnsureCanCreateAsync(command, EntityType.Party, cancellationToken);

    UserId userId = command.GetUserId();
    Party party = new(command.GetWorldId(), new Name(payload.Name), userId)
    {
      Description = Description.TryCreate(payload.Description)
    };

    party.Update(userId);
    await _sender.Send(new SavePartyCommand(party), cancellationToken);

    return await _partyQuerier.ReadAsync(party, cancellationToken);
  }
}
