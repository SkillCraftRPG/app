using Logitar.Portal.Contracts.Search;
using MediatR;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Lineages;

namespace SkillCraft.Application.Lineages.Queries;

public record SearchLineagesQuery(SearchLineagesPayload Payload) : Activity, IRequest<SearchResults<LineageModel>>;

internal class SearchLineagesQueryHandler : IRequestHandler<SearchLineagesQuery, SearchResults<LineageModel>>
{
  private readonly ILineageQuerier _lineageQuerier;
  private readonly IPermissionService _permissionService;

  public SearchLineagesQueryHandler(ILineageQuerier lineageQuerier, IPermissionService permissionService)
  {
    _lineageQuerier = lineageQuerier;
    _permissionService = permissionService;
  }

  public async Task<SearchResults<LineageModel>> Handle(SearchLineagesQuery query, CancellationToken cancellationToken)
  {
    await _permissionService.EnsureCanPreviewAsync(query, EntityType.Lineage, cancellationToken);

    return await _lineageQuerier.SearchAsync(query.GetWorldId(), query.Payload, cancellationToken);
  }
}
