using FluentValidation;
using MediatR;
using SkillCraft.Application.Items.Validators;
using SkillCraft.Application.Permissions;
using SkillCraft.Application.Storages;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Items;
using SkillCraft.Domain;
using SkillCraft.Domain.Items;
using SkillCraft.Domain.Items.Properties;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Items.Commands;

public record CreateOrReplaceItemResult(ItemModel? Item = null, bool Created = false);

/// <exception cref="NotEnoughAvailableStorageException"></exception>
/// <exception cref="PermissionDeniedException"></exception>
/// <exception cref="ValidationException"></exception>
public record CreateOrReplaceItemCommand(Guid? Id, CreateOrReplaceItemPayload Payload, long? Version) : Activity, IRequest<CreateOrReplaceItemResult>;

internal class CreateOrReplaceItemCommandHandler : IRequestHandler<CreateOrReplaceItemCommand, CreateOrReplaceItemResult>
{
  private readonly IItemQuerier _itemQuerier;
  private readonly IItemRepository _itemRepository;
  private readonly IPermissionService _permissionService;
  private readonly ISender _sender;

  public CreateOrReplaceItemCommandHandler(
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

  public async Task<CreateOrReplaceItemResult> Handle(CreateOrReplaceItemCommand command, CancellationToken cancellationToken)
  {
    new CreateOrReplaceItemValidator().ValidateAndThrow(command.Payload);

    Item? item = await FindAsync(command, cancellationToken);
    bool created = false;
    if (item == null)
    {
      if (command.Version.HasValue)
      {
        return new CreateOrReplaceItemResult();
      }

      item = await CreateAsync(command, cancellationToken);
      created = true;
    }
    else
    {
      await ReplaceAsync(command, item, cancellationToken);
    }

    await _sender.Send(new SaveItemCommand(item), cancellationToken);

    ItemModel model = await _itemQuerier.ReadAsync(item, cancellationToken);
    return new CreateOrReplaceItemResult(model, created);
  }

  private async Task<Item?> FindAsync(CreateOrReplaceItemCommand command, CancellationToken cancellationToken)
  {
    if (!command.Id.HasValue)
    {
      return null;
    }

    ItemId id = new(command.GetWorldId(), command.Id.Value);
    return await _itemRepository.LoadAsync(id, cancellationToken);
  }

  private async Task<Item> CreateAsync(CreateOrReplaceItemCommand command, CancellationToken cancellationToken)
  {
    await _permissionService.EnsureCanCreateAsync(command, EntityType.Item, cancellationToken);

    CreateOrReplaceItemPayload payload = command.Payload;
    UserId userId = command.GetUserId();
    WorldId worldId = command.GetWorldId();

    PropertiesBase properties = GetProperties(worldId, payload);
    Item item = new(worldId, new Name(payload.Name), properties, userId, command.Id)
    {
      Description = Description.TryCreate(payload.Description),
      Value = payload.Value,
      Weight = payload.Weight,
      IsAttunementRequired = payload.IsAttunementRequired
    };
    item.Update(userId);

    return item;
  }
  private static PropertiesBase GetProperties(WorldId worldId, CreateOrReplaceItemPayload payload)
  {
    List<PropertiesBase> properties = new(capacity: 7);
    if (payload.Consumable != null)
    {
      properties.Add(payload.Consumable.ToConsumableProperties(worldId)); // TODO(fpion): ensure replacement item exists
    }
    if (payload.Container != null)
    {
      properties.Add(new ContainerProperties(payload.Container));
    }
    if (payload.Device != null)
    {
      properties.Add(new DeviceProperties(payload.Device));
    }
    if (payload.Equipment != null)
    {
      properties.Add(new EquipmentProperties(payload.Equipment));
    }
    if (payload.Miscellaneous != null)
    {
      properties.Add(new MiscellaneousProperties(payload.Miscellaneous));
    }
    if (payload.Money != null)
    {
      properties.Add(new MoneyProperties(payload.Money));
    }
    if (payload.Weapon != null)
    {
      properties.Add(payload.Weapon.ToWeaponProperties());
    }
    return properties.Single();
  }

  private async Task ReplaceAsync(CreateOrReplaceItemCommand command, Item item, CancellationToken cancellationToken)
  {
    await _permissionService.EnsureCanUpdateAsync(command, item.GetMetadata(), cancellationToken);

    CreateOrReplaceItemPayload payload = command.Payload;
    UserId userId = command.GetUserId();

    Item? reference = null;
    if (command.Version.HasValue)
    {
      reference = await _itemRepository.LoadAsync(item.Id, command.Version.Value, cancellationToken);
    }
    reference ??= item;

    Name name = new(payload.Name);
    if (name != reference.Name)
    {
      item.Name = name;
    }
    Description? description = Description.TryCreate(payload.Description);
    if (description != reference.Description)
    {
      item.Description = description;
    }

    if (payload.Value != reference.Value)
    {
      item.Value = payload.Value;
    }
    if (payload.Weight != reference.Weight)
    {
      item.Weight = payload.Weight;
    }

    if (payload.IsAttunementRequired != reference.IsAttunementRequired)
    {
      item.IsAttunementRequired = payload.IsAttunementRequired;
    }

    if (payload.Consumable != null)
    {
      item.SetProperties(payload.Consumable.ToConsumableProperties(item.WorldId), userId); // TODO(fpion): ensure replacement item exists
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
      item.SetProperties(payload.Weapon.ToWeaponProperties(), userId);
    }

    item.Update(userId);
  }
}
