using Logitar.Portal.Contracts.Search;
using MediatR;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Items;

namespace SkillCraft.Application.Items.Queries;

/// <exception cref="PermissionDeniedException"></exception>
public record SearchItemsQuery(SearchItemsPayload Payload) : Activity, IRequest<SearchResults<ItemModel>>;

internal class SearchItemsQueryHandler : IRequestHandler<SearchItemsQuery, SearchResults<ItemModel>>
{
  private readonly IItemQuerier _itemQuerier;
  private readonly IPermissionService _permissionService;

  public SearchItemsQueryHandler(IItemQuerier itemQuerier, IPermissionService permissionService)
  {
    _itemQuerier = itemQuerier;
    _permissionService = permissionService;
  }

  public async Task<SearchResults<ItemModel>> Handle(SearchItemsQuery query, CancellationToken cancellationToken)
  {
    await _permissionService.EnsureCanPreviewAsync(query, EntityType.Item, cancellationToken);

    return await _itemQuerier.SearchAsync(query.GetWorldId(), query.Payload, cancellationToken);
  }
}
