using Moq;
using SkillCraft.Application.Storage;
using SkillCraft.Domain;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Worlds.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class SaveWorldCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IStorageService> _storageService = new();
  private readonly Mock<IWorldRepository> _worldRepository = new();

  private readonly SaveWorldCommandHandler _handler;

  public SaveWorldCommandHandlerTests()
  {
    _handler = new(_storageService.Object, _worldRepository.Object);
  }

  [Fact(DisplayName = "It should save the world when the unique name has not changed.")]
  public async Task It_should_save_the_world_when_the_unique_name_has_not_changed()
  {
    WorldAggregate world = new(new SlugUnit("new-world"));
    world.ClearChanges();

    SaveWorldCommand command = new(world);
    await _handler.Handle(command, _cancellationToken);

    _storageService.Verify(x => x.EnsureAvailableAsync(world, 0, _cancellationToken), Times.Once);
    _worldRepository.Verify(x => x.SaveAsync(world, _cancellationToken), Times.Once);
    _worldRepository.VerifyNoOtherCalls();
  }

  [Fact(DisplayName = "It should save the world when there is no conflict.")]
  public async Task It_should_save_the_world_when_there_is_no_conflict()
  {
    WorldAggregate world = new(new SlugUnit("new-world"));
    _worldRepository.Setup(x => x.LoadAsync(world.UniqueSlug, _cancellationToken)).ReturnsAsync(world);

    SaveWorldCommand command = new(world);
    await _handler.Handle(command, _cancellationToken);

    _storageService.Verify(x => x.EnsureAvailableAsync(world, 0, _cancellationToken), Times.Once);
    _worldRepository.Verify(x => x.SaveAsync(world, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should throw UniqueSlugAlreadyUsedException when the unique slug is already used.")]
  public async Task It_should_throw_UniqueSlugAlreadyUsedException_when_the_unique_slug_is_already_used()
  {
    WorldAggregate world = new(new SlugUnit("new-world"));

    WorldAggregate other = new(world.UniqueSlug);
    _worldRepository.Setup(x => x.LoadAsync(world.UniqueSlug, _cancellationToken)).ReturnsAsync(other);

    SaveWorldCommand command = new(world);
    var exception = await Assert.ThrowsAsync<UniqueSlugAlreadyUsedException>(async () => await _handler.Handle(command, _cancellationToken));
    Assert.Equal(world.UniqueSlug.Value, exception.UniqueSlug);
    Assert.Equal("UniqueSlug", exception.PropertyName);
  }
}
