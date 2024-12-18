﻿using MediatR;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Parties;

namespace SkillCraft.Application.Parties.Queries;

/// <exception cref="PermissionDeniedException"></exception>
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
    await _permissionService.EnsureCanPreviewAsync(query, EntityType.Party, cancellationToken);

    return await _partyQuerier.ReadAsync(query.GetWorldId(), query.Id, cancellationToken);
  }
}
