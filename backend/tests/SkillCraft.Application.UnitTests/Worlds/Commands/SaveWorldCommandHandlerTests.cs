using Moq;
using SkillCraft.Application.Storages;
using SkillCraft.Domain;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Worlds.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class SaveWorldCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IStorageService> _storageService = new();
  private readonly Mock<IWorldQuerier> _worldQuerier = new();
  private readonly Mock<IWorldRepository> _worldRepository = new();

  private readonly SaveWorldCommandHandler _handler;

  public SaveWorldCommandHandlerTests()
  {
    _handler = new(_storageService.Object, _worldQuerier.Object, _worldRepository.Object);
  }

  [Fact(DisplayName = "It should save the world.")]
  public async Task It_should_save_the_world()
  {
    World world = new(new Slug("ungar"), UserId.NewId());

    SaveWorldCommand command = new(world);
    await _handler.Handle(command, _cancellationToken);

    _worldRepository.Verify(x => x.SaveAsync(world, _cancellationToken), Times.Once);

    _storageService.Verify(x => x.EnsureAvailableAsync(world, _cancellationToken), Times.Once);
    _storageService.Verify(x => x.UpdateAsync(world, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should throw SlugAlreadyUsedException when the slug is already used.")]
  public async Task It_should_throw_SlugAlreadyUsedException_when_the_slug_is_already_used()
  {
    World world = new(new Slug("ungar"), UserId.NewId());

    World other = new(world.Slug, world.OwnerId);
    _worldQuerier.Setup(x => x.FindIdAsync(world.Slug, _cancellationToken)).ReturnsAsync(other.Id);

    SaveWorldCommand command = new(world);
    var exception = await Assert.ThrowsAsync<SlugAlreadyUsedException>(async () => await _handler.Handle(command, _cancellationToken));
    Assert.Equal([world.EntityId, other.EntityId], exception.ConflictingIds);
    Assert.Equal(world.Slug.Value, exception.Slug);
    Assert.Equal("Slug", exception.PropertyName);
  }
}
