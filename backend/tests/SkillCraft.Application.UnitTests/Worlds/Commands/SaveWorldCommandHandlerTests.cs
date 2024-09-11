using Moq;
using SkillCraft.Application.Storages;
using SkillCraft.Domain;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Worlds.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class SaveWorldCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;
  private readonly World _world = new(new Slug("new-world"), UserId.NewId());

  private readonly Mock<IStorageService> _storageService = new();
  private readonly Mock<IWorldRepository> _worldRepository = new();

  private readonly SaveWorldCommandHandler _handler;

  public SaveWorldCommandHandlerTests()
  {
    _handler = new(_storageService.Object, _worldRepository.Object);
  }

  [Fact(DisplayName = "It should save the world.")]
  public async Task It_should_save_the_world()
  {
    _worldRepository.Setup(x => x.LoadAsync(_world.Slug, _cancellationToken)).ReturnsAsync(_world);

    SaveWorldCommand command = new(_world);

    await _handler.Handle(command, _cancellationToken);

    _worldRepository.Verify(x => x.SaveAsync(_world, _cancellationToken), Times.Once);

    EntityMetadata entity = EntityMetadata.From(_world);
    _storageService.Verify(x => x.EnsureAvailableAsync(entity, _cancellationToken), Times.Once);
    _storageService.Verify(x => x.UpdateAsync(entity, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should throw SlugAlreadyUsedException when the slug is already used.")]
  public async Task It_should_throw_SlugAlreadyUsedException_when_the_slug_is_already_used()
  {
    World world = new(_world.Slug, _world.OwnerId);
    _worldRepository.Setup(x => x.LoadAsync(_world.Slug, _cancellationToken)).ReturnsAsync(world);

    SaveWorldCommand command = new(_world);

    var exception = await Assert.ThrowsAsync<SlugAlreadyUsedException>(async () => await _handler.Handle(command, _cancellationToken));
    Assert.Equal(world.Slug.Value, exception.Slug);
    Assert.Equal("Slug", exception.PropertyName);
  }
}
