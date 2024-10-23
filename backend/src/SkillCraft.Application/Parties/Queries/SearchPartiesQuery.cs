using Logitar.Portal.Contracts.Search;
using MediatR;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Parties;

namespace SkillCraft.Application.Parties.Queries;

/// <exception cref="PermissionDeniedException"></exception>
public record SearchPartiesQuery(SearchPartiesPayload Payload) : Activity, IRequest<SearchResults<PartyModel>>;

internal class SearchPartiesQueryHandler : IRequestHandler<SearchPartiesQuery, SearchResults<PartyModel>>
{
  private readonly IPartyQuerier _partyQuerier;
  private readonly IPermissionService _permissionService;

  public SearchPartiesQueryHandler(IPartyQuerier partyQuerier, IPermissionService permissionService)
  {
    _partyQuerier = partyQuerier;
    _permissionService = permissionService;
  }

  public async Task<SearchResults<PartyModel>> Handle(SearchPartiesQuery query, CancellationToken cancellationToken)
  {
    await _permissionService.EnsureCanPreviewAsync(query, EntityType.Party, cancellationToken);

    return await _partyQuerier.SearchAsync(query.GetWorldId(), query.Payload, cancellationToken);
  }
}
