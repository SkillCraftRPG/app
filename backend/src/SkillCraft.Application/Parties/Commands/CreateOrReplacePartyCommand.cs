using FluentValidation;
using MediatR;
using SkillCraft.Application.Parties.Validators;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Parties;
using SkillCraft.Domain;
using SkillCraft.Domain.Parties;

namespace SkillCraft.Application.Parties.Commands;

public record CreateOrReplacePartyResult(PartyModel? Party = null, bool Created = false);

public record CreateOrReplacePartyCommand(Guid? Id, CreateOrReplacePartyPayload Payload, long? Version) : Activity, IRequest<CreateOrReplacePartyResult>;

internal class CreateOrReplacePartyCommandHandler : IRequestHandler<CreateOrReplacePartyCommand, CreateOrReplacePartyResult>
{
  private readonly IPartyQuerier _partyQuerier;
  private readonly IPartyRepository _partyRepository;
  private readonly IPermissionService _permissionService;
  private readonly ISender _sender;

  public CreateOrReplacePartyCommandHandler(
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

  public async Task<CreateOrReplacePartyResult> Handle(CreateOrReplacePartyCommand command, CancellationToken cancellationToken)
  {
    new CreateOrReplacePartyValidator().ValidateAndThrow(command.Payload);

    Party? party = await FindAsync(command, cancellationToken);
    bool created = false;
    if (party == null)
    {
      if (command.Version.HasValue)
      {
        return new CreateOrReplacePartyResult();
      }

      party = await CreateAsync(command, cancellationToken);
      created = true;
    }
    else
    {
      await ReplaceAsync(command, party, cancellationToken);
    }

    await _sender.Send(new SavePartyCommand(party), cancellationToken);

    PartyModel model = await _partyQuerier.ReadAsync(party, cancellationToken);
    return new CreateOrReplacePartyResult(model, created);
  }

  private async Task<Party?> FindAsync(CreateOrReplacePartyCommand command, CancellationToken cancellationToken)
  {
    if (!command.Id.HasValue)
    {
      return null;
    }

    PartyId id = new(command.GetWorldId(), command.Id.Value);
    return await _partyRepository.LoadAsync(id, cancellationToken);
  }

  private async Task<Party> CreateAsync(CreateOrReplacePartyCommand command, CancellationToken cancellationToken)
  {
    await _permissionService.EnsureCanCreateAsync(command, EntityType.Party, cancellationToken);

    CreateOrReplacePartyPayload payload = command.Payload;
    UserId userId = command.GetUserId();

    Party party = new(command.GetWorldId(), new Name(payload.Name), userId, command.Id)
    {
      Description = Description.TryCreate(payload.Description)
    };
    party.Update(userId);

    return party;
  }

  private async Task ReplaceAsync(CreateOrReplacePartyCommand command, Party party, CancellationToken cancellationToken)
  {
    await _permissionService.EnsureCanUpdateAsync(command, party.GetMetadata(), cancellationToken);

    CreateOrReplacePartyPayload payload = command.Payload;
    UserId userId = command.GetUserId();

    Party? reference = null;
    if (command.Version.HasValue)
    {
      reference = await _partyRepository.LoadAsync(party.Id, command.Version.Value, cancellationToken);
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

    party.Update(userId);
  }
}
