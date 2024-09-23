using MediatR;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts.Parties;

namespace SkillCraft.Application.Parties.Queries;

public record ReadPartyQuery(Guid Id) : Activity, IRequest<PartyModel?>;

internal class ReadPartyQueryHandler : IRequestHandler<ReadPartyQuery, PartyModel?>
{
  private readonly IPartyQuerier _partyQuerier;
  private readonly IPermissionService _permissionService;

  public ReadPartyQueryHandler(IPartyQuerier partyQuerier, IPermissionService permissionService)
  {
    _partyQuerier = partyQuerier;
    _permissionService = permissionService;
  }

  public async Task<PartyModel?> Handle(ReadPartyQuery query, CancellationToken cancellationToken)
  {
    PartyModel? party = await _partyQuerier.ReadAsync(query.Id, cancellationToken);
    if (party != null)
    {
      await _permissionService.EnsureCanPreviewAsync(query, party.GetMetadata(), cancellationToken);
    }

    return party;
  }
}
