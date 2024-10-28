using Logitar.Portal.Contracts.Actors;
using Moq;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Items;
using SkillCraft.Contracts.Worlds;

namespace SkillCraft.Application.Items.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class ReadItemQueryHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IItemQuerier> _itemQuerier = new();
  private readonly Mock<IPermissionService> _permissionService = new();

  private readonly ReadItemQueryHandler _handler;

  private readonly UserMock _user = new();
  private readonly WorldMock _world;
  private readonly ItemModel _item;

  public ReadItemQueryHandlerTests()
  {
    _handler = new(_itemQuerier.Object, _permissionService.Object);

    _world = new(_user);
    WorldModel worldModel = new(new Actor(_user), _world.Slug.Value)
    {
      Id = _world.Id.ToGuid()
    };
    _item = new(worldModel, "Denier")
    {
      Id = Guid.NewGuid()
    };
    _itemQuerier.Setup(x => x.ReadAsync(_world.Id, _item.Id, _cancellationToken)).ReturnsAsync(_item);
  }

  [Fact(DisplayName = "It should return null when no item is found.")]
  public async Task It_should_return_null_when_no_item_is_found()
  {
    ReadItemQuery query = new(Guid.Empty);
    query.Contextualize(_world);

    Assert.Null(await _handler.Handle(query, _cancellationToken));

    _permissionService.Verify(x => x.EnsureCanPreviewAsync(query, EntityType.Item, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should return the item found by ID.")]
  public async Task It_should_return_the_item_found_by_Id()
  {
    ReadItemQuery query = new(_item.Id);
    query.Contextualize(_world);

    var item = await _handler.Handle(query, _cancellationToken);
    Assert.Same(_item, item);

    _permissionService.Verify(x => x.EnsureCanPreviewAsync(query, EntityType.Item, _cancellationToken), Times.Once);
  }
}
