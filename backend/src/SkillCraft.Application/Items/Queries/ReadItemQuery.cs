using MediatR;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Items;

namespace SkillCraft.Application.Items.Queries;

/// <exception cref="PermissionDeniedException"></exception>
public record ReadItemQuery(Guid Id) : Activity, IRequest<ItemModel?>;

internal class ReadItemQueryHandler : IRequestHandler<ReadItemQuery, ItemModel?>
{
  private readonly IItemQuerier _itemQuerier;
  private readonly IPermissionService _permissionService;

  public ReadItemQueryHandler(IItemQuerier itemQuerier, IPermissionService permissionService)
  {
    _itemQuerier = itemQuerier;
    _permissionService = permissionService;
  }

  public async Task<ItemModel?> Handle(ReadItemQuery query, CancellationToken cancellationToken)
  {
    await _permissionService.EnsureCanPreviewAsync(query, EntityType.Item, cancellationToken);

    return await _itemQuerier.ReadAsync(query.GetWorldId(), query.Id, cancellationToken);
  }
}
