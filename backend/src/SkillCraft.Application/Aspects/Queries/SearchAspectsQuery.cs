using Logitar.Portal.Contracts.Search;
using MediatR;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts.Aspects;
using SkillCraft.Domain;

namespace SkillCraft.Application.Aspects.Queries;

public record SearchAspectsQuery(SearchAspectsPayload Payload) : Activity, IRequest<SearchResults<AspectModel>>;

internal class SearchAspectsQueryHandler : IRequestHandler<SearchAspectsQuery, SearchResults<AspectModel>>
{
  private readonly IAspectQuerier _aspectQuerier;
  private readonly IPermissionService _permissionService;

  public SearchAspectsQueryHandler(IAspectQuerier aspectQuerier, IPermissionService permissionService)
  {
    _aspectQuerier = aspectQuerier;
    _permissionService = permissionService;
  }

  public async Task<SearchResults<AspectModel>> Handle(SearchAspectsQuery query, CancellationToken cancellationToken)
  {
    await _permissionService.EnsureCanPreviewAsync(query, EntityType.Aspect, cancellationToken);

    return await _aspectQuerier.SearchAsync(query.GetWorldId(), query.Payload, cancellationToken);
  }
}
