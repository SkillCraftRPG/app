using Logitar.Portal.Contracts.Search;
using MediatR;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Castes;

namespace SkillCraft.Application.Castes.Queries;

public record SearchCastesQuery(SearchCastesPayload Payload) : Activity, IRequest<SearchResults<CasteModel>>;

internal class SearchCastesQueryHandler : IRequestHandler<SearchCastesQuery, SearchResults<CasteModel>>
{
  private readonly ICasteQuerier _casteQuerier;
  private readonly IPermissionService _permissionService;

  public SearchCastesQueryHandler(ICasteQuerier casteQuerier, IPermissionService permissionService)
  {
    _casteQuerier = casteQuerier;
    _permissionService = permissionService;
  }

  public async Task<SearchResults<CasteModel>> Handle(SearchCastesQuery query, CancellationToken cancellationToken)
  {
    await _permissionService.EnsureCanPreviewAsync(query, EntityType.Caste, cancellationToken);

    return await _casteQuerier.SearchAsync(query.GetWorldId(), query.Payload, cancellationToken);
  }
}
