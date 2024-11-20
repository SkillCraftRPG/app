using Logitar.Portal.Contracts.Search;
using MediatR;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Natures;

namespace SkillCraft.Application.Natures.Queries;

/// <exception cref="PermissionDeniedException"></exception>
public record SearchPersonalitiesQuery(SearchNaturesPayload Payload) : Activity, IRequest<SearchResults<NatureModel>>;

internal class SearchPersonalitiesQueryHandler : IRequestHandler<SearchPersonalitiesQuery, SearchResults<NatureModel>>
{
  private readonly INatureQuerier _natureQuerier;
  private readonly IPermissionService _permissionService;

  public SearchPersonalitiesQueryHandler(INatureQuerier natureQuerier, IPermissionService permissionService)
  {
    _natureQuerier = natureQuerier;
    _permissionService = permissionService;
  }

  public async Task<SearchResults<NatureModel>> Handle(SearchPersonalitiesQuery query, CancellationToken cancellationToken)
  {
    await _permissionService.EnsureCanPreviewAsync(query, EntityType.Nature, cancellationToken);

    return await _natureQuerier.SearchAsync(query.GetWorldId(), query.Payload, cancellationToken);
  }
}
