using Moq;
using SkillCraft.Application.Characters.Commands;
using SkillCraft.Application.Items;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Characters;
using SkillCraft.Domain;
using SkillCraft.Domain.Items;
using SkillCraft.Domain.Items.Properties;

namespace SkillCraft.Application.Characters.Creation;

[Trait(Traits.Category, Categories.Unit)]
public class ResolveItemQueryHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IItemRepository> _itemRepository = new();
  private readonly Mock<IPermissionService> _permissionService = new();

  private readonly ResolveItemQueryHandler _handler;

  private readonly WorldMock _world = new();
  private readonly Item _denier;
  private readonly Item _dime;
  private readonly Item _goldOre;
  private readonly CreateCharacterCommand _activity = new(new CreateCharacterPayload());

  public ResolveItemQueryHandlerTests()
  {
    _handler = new(_itemRepository.Object, _permissionService.Object);

    _denier = new(_world.Id, new Name("Denier"), new MoneyProperties(), _world.OwnerId)
    {
      Value = 1.0,
      Weight = 0.02
    };
    _denier.Update(_world.OwnerId);
    _itemRepository.Setup(x => x.LoadAsync(_denier.Id, _cancellationToken)).ReturnsAsync(_denier);

    _dime = new(_world.Id, new Name("Dîme"), new MoneyProperties(), _world.OwnerId)
    {
      Value = 0.1,
      Weight = 0.02
    };
    _dime.Update(_world.OwnerId);
    _itemRepository.Setup(x => x.LoadAsync(_dime.Id, _cancellationToken)).ReturnsAsync(_dime);

    _goldOre = new(_world.Id, new Name("Pépite d’or"), new MiscellaneousProperties(), _world.OwnerId)
    {
      Value = 1.0,
      Weight = 0.1
    };
    _goldOre.Update(_world.OwnerId);
    _itemRepository.Setup(x => x.LoadAsync(_goldOre.Id, _cancellationToken)).ReturnsAsync(_goldOre);

    _activity.Contextualize(_world);
  }

  [Fact(DisplayName = "It should return the found item.")]
  public async Task It_should_return_the_found_item()
  {
    ResolveItemQuery query = new(_activity, _denier.EntityId);

    Item item = await _handler.Handle(query, _cancellationToken);
    Assert.Same(_denier, item);

    _permissionService.Verify(x => x.EnsureCanPreviewAsync(_activity, EntityType.Item, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should throw InvalidStartingWealthSelectionException when the item is not money.")]
  public async Task It_should_throw_InvalidStartingWealthSelectionException_when_the_item_is_not_money()
  {
    ResolveItemQuery query = new(_activity, _goldOre.EntityId);

    var exception = await Assert.ThrowsAsync<InvalidStartingWealthSelectionException>(async () => await _handler.Handle(query, _cancellationToken));
    Assert.Equal(_world.Id.ToGuid(), exception.WorldId);
    Assert.Equal(_goldOre.EntityId, exception.ItemId);
    Assert.Equal(_goldOre.Category, exception.ItemCategory);
    Assert.Equal(_goldOre.Value, exception.ItemValue);
    Assert.Equal("StartingWealth.ItemId", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw InvalidStartingWealthSelectionException when the item value is not 1.")]
  public async Task It_should_throw_InvalidStartingWealthSelectionException_when_the_item_value_is_not_1()
  {
    ResolveItemQuery query = new(_activity, _dime.EntityId);

    var exception = await Assert.ThrowsAsync<InvalidStartingWealthSelectionException>(async () => await _handler.Handle(query, _cancellationToken));
    Assert.Equal(_world.Id.ToGuid(), exception.WorldId);
    Assert.Equal(_dime.EntityId, exception.ItemId);
    Assert.Equal(_dime.Category, exception.ItemCategory);
    Assert.Equal(_dime.Value, exception.ItemValue);
    Assert.Equal("StartingWealth.ItemId", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw ItemNotFoundException when the item could not be found.")]
  public async Task It_should_throw_ItemNotFoundException_when_the_item_could_not_be_found()
  {
    ResolveItemQuery query = new(_activity, Guid.NewGuid());

    var exception = await Assert.ThrowsAsync<ItemNotFoundException>(async () => await _handler.Handle(query, _cancellationToken));
    Assert.Equal(_world.Id.ToGuid(), exception.WorldId);
    Assert.Equal(query.Id, exception.ItemId);
    Assert.Equal("StartingWealth.ItemId", exception.PropertyName);
  }
}
