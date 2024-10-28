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
public class UpdateItemCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IItemQuerier> _itemQuerier = new();
  private readonly Mock<IItemRepository> _itemRepository = new();
  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<ISender> _sender = new();

  private readonly UpdateItemCommandHandler _handler;

  private readonly WorldMock _world = new();

  public UpdateItemCommandHandlerTests()
  {
    _handler = new(_itemQuerier.Object, _itemRepository.Object, _permissionService.Object, _sender.Object);
  }

  [Fact(DisplayName = "It should return null when the item could not be found.")]
  public async Task It_should_return_null_when_the_item_could_not_be_found()
  {
    UpdateItemPayload payload = new();
    UpdateItemCommand command = new(Guid.Empty, payload);
    command.Contextualize(_world);

    Assert.Null(await _handler.Handle(command, _cancellationToken));
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    Item item = new(_world.Id, new Name("denier"), new MoneyProperties(), _world.OwnerId)
    {
      Description = new Description("Le denier, une pièce d’argent couramment utilisée par la plupart des membres de la population pour leurs achats de plus grande valeur, comme le bétail de moyens de transport. Il s’agit de l’unité de référence de ce système."),
      IsAttunementRequired = true
    };
    item.Update(_world.OwnerId);
    _itemRepository.Setup(x => x.LoadAsync(item.Id, _cancellationToken)).ReturnsAsync(item);

    UpdateItemPayload payload = new()
    {
      Value = new Change<double?>(-0.1),
      Weapon = new WeaponPropertiesModel()
    };
    UpdateItemCommand command = new(item.EntityId, payload);
    command.Contextualize(_world);

    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.Handle(command, _cancellationToken));
    Assert.Equal(2, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "GreaterThanOrEqualValidator" && e.PropertyName == "Value.Value");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NullValidator" && e.PropertyName == "Weapon");

    _permissionService.Verify(x => x.EnsureCanUpdateAsync(command,
      It.Is<EntityMetadata>(y => y.WorldId == _world.Id && y.Type == EntityType.Item && y.Id == item.EntityId && y.Size > 0),
      _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should update an existing item.")]
  public async Task It_should_update_an_existing_item()
  {
    Item item = new(_world.Id, new Name("denier"), new MoneyProperties(), _world.OwnerId)
    {
      Description = new Description("Le denier, une pièce d’argent couramment utilisée par la plupart des membres de la population pour leurs achats de plus grande valeur, comme le bétail de moyens de transport. Il s’agit de l’unité de référence de ce système."),
      IsAttunementRequired = true
    };
    item.Update(_world.OwnerId);
    _itemRepository.Setup(x => x.LoadAsync(item.Id, _cancellationToken)).ReturnsAsync(item);

    UpdateItemPayload payload = new()
    {
      Name = " Denier ",
      Description = new Change<string>("    "),
      Value = new Change<double?>(1.0),
      Weight = new Change<double?>(0.02),
      IsAttunementRequired = false
    };
    UpdateItemCommand command = new(item.EntityId, payload);
    command.Contextualize(_world);

    ItemModel model = new();
    _itemQuerier.Setup(x => x.ReadAsync(item, _cancellationToken)).ReturnsAsync(model);

    var result = await _handler.Handle(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(model, result);

    _permissionService.Verify(x => x.EnsureCanUpdateAsync(command,
      It.Is<EntityMetadata>(y => y.WorldId == _world.Id && y.Type == EntityType.Item && y.Id == item.EntityId && y.Size > 0),
      _cancellationToken), Times.Once);

    _sender.Verify(x => x.Send(
      It.Is<SaveItemCommand>(y => y.Item.Equals(item)
        && y.Item.Name.Value == payload.Name.Trim()
        && y.Item.Description == null
        && y.Item.Value == payload.Value.Value
        && y.Item.Weight == payload.Weight.Value
        && y.Item.IsAttunementRequired == payload.IsAttunementRequired),
      _cancellationToken), Times.Once);
  }
}
