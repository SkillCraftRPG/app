using FluentValidation;
using MediatR;
using SkillCraft.Application.Items.Validators;
using SkillCraft.Application.Permissions;
using SkillCraft.Application.Storages;
using SkillCraft.Contracts.Items;
using SkillCraft.Domain;
using SkillCraft.Domain.Items;
using SkillCraft.Domain.Items.Properties;

namespace SkillCraft.Application.Items.Commands;

/// <exception cref="NotEnoughAvailableStorageException"></exception>
/// <exception cref="PermissionDeniedException"></exception>
/// <exception cref="ValidationException"></exception>
public record UpdateItemCommand(Guid Id, UpdateItemPayload Payload) : Activity, IRequest<ItemModel?>;

internal class UpdateItemCommandHandler : IRequestHandler<UpdateItemCommand, ItemModel?>
{
  private readonly IItemQuerier _itemQuerier;
  private readonly IItemRepository _itemRepository;
  private readonly IPermissionService _permissionService;
  private readonly ISender _sender;

  public UpdateItemCommandHandler(
    IItemQuerier itemQuerier,
    IItemRepository itemRepository,
    IPermissionService permissionService,
    ISender sender)
  {
    _itemQuerier = itemQuerier;
    _itemRepository = itemRepository;
    _permissionService = permissionService;
    _sender = sender;
  }

  public async Task<ItemModel?> Handle(UpdateItemCommand command, CancellationToken cancellationToken)
  {
    ItemId id = new(command.GetWorldId(), command.Id);
    Item? item = await _itemRepository.LoadAsync(id, cancellationToken);
    if (item == null)
    {
      return null;
    }

    await _permissionService.EnsureCanUpdateAsync(command, item.GetMetadata(), cancellationToken);

    UpdateItemPayload payload = command.Payload;
    new UpdateItemValidator(item.Category).ValidateAndThrow(payload);

    UserId userId = command.GetUserId();

    if (!string.IsNullOrWhiteSpace(payload.Name))
    {
      item.Name = new Name(payload.Name);
    }
    if (payload.Description != null)
    {
      item.Description = Description.TryCreate(payload.Description.Value);
    }

    if (payload.Value != null)
    {
      item.Value = payload.Value.Value;
    }
    if (payload.Weight != null)
    {
      item.Weight = payload.Weight.Value;
    }

    if (payload.Consumable != null)
    {
      item.SetProperties(new ConsumableProperties(payload.Consumable), userId); // TODO(fpion): ensure replacement item exists
    }
    if (payload.Container != null)
    {
      item.SetProperties(new ContainerProperties(payload.Container), userId);
    }
    if (payload.Device != null)
    {
      item.SetProperties(new DeviceProperties(payload.Device), userId);
    }
    if (payload.Equipment != null)
    {
      item.SetProperties(new EquipmentProperties(payload.Equipment), userId);
    }
    if (payload.Miscellaneous != null)
    {
      item.SetProperties(new MiscellaneousProperties(payload.Miscellaneous), userId);
    }
    if (payload.Money != null)
    {
      item.SetProperties(new MoneyProperties(payload.Money), userId);
    }
    if (payload.Weapon != null)
    {
      item.SetProperties(new WeaponProperties(payload.Weapon), userId);
    }

    item.Update(userId);

    await _sender.Send(new SaveItemCommand(item), cancellationToken);

    return await _itemQuerier.ReadAsync(item, cancellationToken);
  }
}
