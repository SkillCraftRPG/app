using Moq;
using SkillCraft.Application.Storages;
using SkillCraft.Domain;
using SkillCraft.Domain.Items;
using SkillCraft.Domain.Items.Properties;

namespace SkillCraft.Application.Items.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class SaveItemCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IItemRepository> _itemRepository = new();
  private readonly Mock<IStorageService> _storageService = new();

  private readonly SaveItemCommandHandler _handler;

  public SaveItemCommandHandlerTests()
  {
    _handler = new(_itemRepository.Object, _storageService.Object);
  }

  [Fact(DisplayName = "It should save the item.")]
  public async Task It_should_save_the_item()
  {
    WorldMock world = new();
    Item item = new(world.Id, new Name("Denier"), new MoneyProperties(), world.OwnerId);

    SaveItemCommand command = new(item);
    await _handler.Handle(command, _cancellationToken);

    _itemRepository.Verify(x => x.SaveAsync(item, _cancellationToken), Times.Once);

    EntityMetadata entity = item.GetMetadata();
    _storageService.Verify(x => x.EnsureAvailableAsync(entity, _cancellationToken), Times.Once);
    _storageService.Verify(x => x.UpdateAsync(entity, _cancellationToken), Times.Once);
  }
}
