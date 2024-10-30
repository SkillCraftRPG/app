using MediatR;
using SkillCraft.Application.Items;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Characters;
using SkillCraft.Contracts.Items;
using SkillCraft.Domain.Items;

namespace SkillCraft.Application.Characters.Creation;

internal record ResolveItemQuery(Activity Activity, Guid Id) : IRequest<Item>;

internal class ResolveItemQueryHandler : IRequestHandler<ResolveItemQuery, Item>
{
  private readonly IItemRepository _itemRepository;
  private readonly IPermissionService _permissionService;

  private static string PropertyName => string.Join('.', nameof(CreateCharacterPayload.StartingWealth), nameof(CreateCharacterPayload.StartingWealth.ItemId));

  public ResolveItemQueryHandler(IItemRepository itemRepository, IPermissionService permissionService)
  {
    _itemRepository = itemRepository;
    _permissionService = permissionService;
  }

  public async Task<Item> Handle(ResolveItemQuery query, CancellationToken cancellationToken)
  {
    Activity activity = query.Activity;
    await _permissionService.EnsureCanPreviewAsync(activity, EntityType.Item, cancellationToken);

    ItemId id = new(activity.GetWorldId(), query.Id);
    Item item = await _itemRepository.LoadAsync(id, cancellationToken) ?? throw new ItemNotFoundException(id, PropertyName);
    if (item.Category != ItemCategory.Money || item.Value != 1.0)
    {
      throw new InvalidStartingWealthSelectionException(item, PropertyName);
    }

    return item;
  }
}
