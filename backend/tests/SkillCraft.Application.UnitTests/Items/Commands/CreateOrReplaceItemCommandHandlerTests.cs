using FluentValidation;
using MediatR;
using Moq;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Items;
using SkillCraft.Contracts.Items.Properties;
using SkillCraft.Domain;
using SkillCraft.Domain.Items;
using SkillCraft.Domain.Items.Properties;

namespace SkillCraft.Application.Items.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class CreateOrReplaceItemCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IItemQuerier> _itemQuerier = new();
  private readonly Mock<IItemRepository> _itemRepository = new();
  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<ISender> _sender = new();

  private readonly CreateOrReplaceItemCommandHandler _handler;

  private readonly WorldMock _world = new();
  private readonly Item _item;
  private readonly ItemModel _model = new();

  public CreateOrReplaceItemCommandHandlerTests()
  {
    _handler = new(_itemQuerier.Object, _itemRepository.Object, _permissionService.Object, _sender.Object);

    _item = new(_world.Id, new Name("denier"), new MoneyProperties(), _world.OwnerId)
    {
      IsAttunementRequired = true
    };
    _item.Update(_world.OwnerId);
    _itemRepository.Setup(x => x.LoadAsync(_item.Id, _cancellationToken)).ReturnsAsync(_item);

    _itemQuerier.Setup(x => x.ReadAsync(It.IsAny<Item>(), _cancellationToken)).ReturnsAsync(_model);
  }

  [Theory(DisplayName = "It should create a new item.")]
  [InlineData(null)]
  [InlineData("a089b7ad-9cc1-46e9-91c9-d594cd553bd2")]
  public async Task It_should_create_a_new_item(string? idValue)
  {
    CreateOrReplaceItemPayload payload = new(" Poivrière ")
    {
      Description = "  Un pistolet doté d’un barillet multiple pouvant contenir jusqu’à 6 balles.  ",
      Value = 450.0,
      Weight = 2.5,
      IsAttunementRequired = false,
      Weapon = new WeaponPropertiesModel
      {
        Attack = 5,
        Resistance = 24,
        Traits = [WeaponTrait.Ammunition, WeaponTrait.Loading, WeaponTrait.Range, WeaponTrait.Reload],
        Damages =
        [
          new WeaponDamageModel
          {
            Roll = "1d10",
            Type = DamageType.Piercing
          }
        ],
        Range = new WeaponRangeModel
        {
          Normal = 8,
          Long = 24
        },
        ReloadCount = 6
      }
    };

    bool parsed = Guid.TryParse(idValue, out Guid id);
    CreateOrReplaceItemCommand command = new(parsed ? id : null, payload, Version: null);
    command.Contextualize(_world);

    CreateOrReplaceItemResult result = await _handler.Handle(command, _cancellationToken);
    Assert.Same(_model, result.Item);
    Assert.True(result.Created);

    _permissionService.Verify(x => x.EnsureCanCreateAsync(command, EntityType.Item, _cancellationToken), Times.Once);

    _sender.Verify(x => x.Send(
      It.Is<SaveItemCommand>(y => (!parsed || y.Item.EntityId == id)
        && y.Item.Name.Value == payload.Name.Trim()
        && y.Item.Description != null && y.Item.Description.Value == payload.Description.Trim()
        && y.Item.Value == payload.Value
        && y.Item.Weight == payload.Weight
        && y.Item.IsAttunementRequired == payload.IsAttunementRequired), // TODO(fpion): properties
      _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should replace an existing item.")]
  public async Task It_should_replace_an_existing_item()
  {
    CreateOrReplaceItemPayload payload = new(" Denier ")
    {
      Description = "    ",
      Value = 1.0,
      Weight = 0.02,
      IsAttunementRequired = false,
      Money = new MoneyPropertiesModel()
    };

    CreateOrReplaceItemCommand command = new(_item.EntityId, payload, Version: null);
    command.Contextualize(_world);

    CreateOrReplaceItemResult result = await _handler.Handle(command, _cancellationToken);
    Assert.Same(_model, result.Item);
    Assert.False(result.Created);

    _permissionService.Verify(x => x.EnsureCanUpdateAsync(
      command,
      It.Is<EntityMetadata>(y => y.WorldId == _world.Id && y.Type == EntityType.Item && y.Id == _item.EntityId && y.Size > 0),
      _cancellationToken), Times.Once);

    _sender.Verify(x => x.Send(
      It.Is<SaveItemCommand>(y => y.Item.Equals(_item)
        && y.Item.Name.Value == payload.Name.Trim()
        && y.Item.Description == null
        && y.Item.Value == payload.Value
        && y.Item.Weight == payload.Weight
        && y.Item.IsAttunementRequired == payload.IsAttunementRequired),
      _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should return null when updating a item that does not exist.")]
  public async Task It_should_return_null_when_updating_an_item_that_does_not_exist()
  {
    CreateOrReplaceItemCommand command = new(Guid.Empty, new CreateOrReplaceItemPayload("Denier"), Version: 0);
    command.Contextualize(_world);

    CreateOrReplaceItemResult result = await _handler.Handle(command, _cancellationToken);
    Assert.Null(result.Item);
  }

  [Theory(DisplayName = "It should throw ItemNotFoundException when the consumable replacement item could not be found.")]
  [InlineData(null)]
  [InlineData("4d3d3836-cec3-4c4b-9197-a8ec70789e47")]
  public async Task It_should_throw_ItemNotFoundException_when_the_consumable_replacement_item_could_not_be_found(string? itemId)
  {
    Guid? entityId = itemId == null ? null : Guid.Parse(itemId);
    if (entityId.HasValue)
    {
      Item item = new(_world.Id, new Name("Potion de Vitalité"), new ConsumableProperties(charges: 1, removeWhenEmpty: true, replaceWithItemWhenEmptyId: null), _world.OwnerId, entityId);
      _itemRepository.Setup(x => x.LoadAsync(item.Id, _cancellationToken)).ReturnsAsync(item);
    }

    CreateOrReplaceItemPayload payload = new("Potion de Vitalité")
    {
      Consumable = new ConsumablePropertiesModel
      {
        Charges = 1,
        RemoveWhenEmpty = false,
        ReplaceWithItemWhenEmptyId = Guid.NewGuid()
      }
    };
    CreateOrReplaceItemCommand command = new(entityId, payload, Version: null);
    command.Contextualize(_world);

    var exception = await Assert.ThrowsAsync<ItemNotFoundException>(async () => await _handler.Handle(command, _cancellationToken));
    Assert.Equal(_world.Id.ToGuid(), exception.WorldId);
    Assert.Equal(payload.Consumable.ReplaceWithItemWhenEmptyId, exception.ItemId);
    Assert.Equal("Consumable.ReplaceWithItemWhenEmptyId", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    CreateOrReplaceItemPayload payload = new();
    CreateOrReplaceItemCommand command = new(_item.EntityId, payload, Version: null);
    command.Contextualize(_world);

    var exception = await Assert.ThrowsAsync<ValidationException>(async () => await _handler.Handle(command, _cancellationToken));

    Assert.Equal(2, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "Name");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotNullValidator" && e.PropertyName == "Money");
  }

  [Fact(DisplayName = "It should update an existing item.")]
  public async Task It_should_update_an_existing_item()
  {
    Item reference = new(_item.WorldId, _item.Name, _item.Properties, _world.OwnerId, _item.EntityId)
    {
      Description = _item.Description,
      Value = _item.Value,
      Weight = _item.Weight,
      IsAttunementRequired = _item.IsAttunementRequired
    };
    reference.Update(_world.OwnerId);
    _itemRepository.Setup(x => x.LoadAsync(reference.Id, reference.Version, _cancellationToken)).ReturnsAsync(reference);

    Description description = new("Le dernier, une pièce d’argent couramment utilisée par la plupart des membres de la population pour leurs achats de plus grande valeur, comme le bétail de moyens de transport. Il s’agit de l’unité de référence de ce système.");
    _item.Description = description;
    _item.Update(_world.OwnerId);

    CreateOrReplaceItemPayload payload = new(" Denier ")
    {
      Description = "    ",
      Value = 1.0,
      Weight = 0.02,
      Money = new MoneyPropertiesModel()
    };

    CreateOrReplaceItemCommand command = new(_item.EntityId, payload, reference.Version);
    command.Contextualize(_world);

    CreateOrReplaceItemResult result = await _handler.Handle(command, _cancellationToken);
    Assert.Same(_model, result.Item);
    Assert.False(result.Created);

    _permissionService.Verify(x => x.EnsureCanUpdateAsync(
      command,
      It.Is<EntityMetadata>(y => y.WorldId == _world.Id && y.Type == EntityType.Item && y.Id == _item.EntityId && y.Size > 0),
      _cancellationToken), Times.Once);

    _sender.Verify(x => x.Send(
      It.Is<SaveItemCommand>(y => y.Item.Equals(_item)
        && y.Item.Name.Value == payload.Name.Trim()
        && y.Item.Description == description
        && y.Item.Value == payload.Value
        && y.Item.Weight == payload.Weight
        && y.Item.IsAttunementRequired == payload.IsAttunementRequired),
      _cancellationToken), Times.Once);
  }
}
