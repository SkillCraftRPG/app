using Logitar.Portal.Contracts.Actors;
using Moq;
using SkillCraft.Application.Settings;
using SkillCraft.Application.Worlds;
using SkillCraft.Contracts.Worlds;
using SkillCraft.Domain;
using SkillCraft.Domain.Storages;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Storages;

[Trait(Traits.Category, Categories.Unit)]
public class StorageServiceTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly AccountSettings _accountSettings = new()
  {
    AllocatedBytes = 5 * 1024 * 1024 // 5 MB
  };
  private readonly Mock<IStorageRepository> _storageRepository = new();
  private readonly Mock<IWorldQuerier> _worldQuerier = new();

  private readonly StorageService _service;

  private readonly UserMock _user = new();
  private readonly World _world;
  private readonly WorldModel _worldModel;

  public StorageServiceTests()
  {
    _service = new(_accountSettings, _storageRepository.Object, _worldQuerier.Object);

    _world = new(new Slug("ungar"), new UserId(_user.Id));
    _worldModel = new(new Actor(_user), _world.Slug.Value)
    {
      Id = _world.Id.ToGuid()
    };
    _worldQuerier.Setup(x => x.ReadAsync(_world.Id, _cancellationToken)).ReturnsAsync(_worldModel);
  }

  [Fact(DisplayName = "EnsureAvailableAsync(EntityMetadata): it should succeed when there is enough storage.")]
  public async Task EnsureAvailableAsyncEntityMetadata_it_should_succeed_when_there_is_enough_storage()
  {
    EntityMetadata entity = _world.GetMetadata();

    await _service.EnsureAvailableAsync(entity, _cancellationToken);
  }

  [Fact(DisplayName = "EnsureAvailableAsync(World): it should succeed when there is enough storage.")]
  public async Task EnsureAvailableAsyncWorld_it_should_succeed_when_there_is_enough_storage()
  {
    World world = new(_world.Slug, _world.OwnerId);
    await _service.EnsureAvailableAsync(world, _cancellationToken);
  }

  [Fact(DisplayName = "EnsureAvailableAsync(EntityMetadata): it should throw NotEnoughAvailableStorageException when there is not enough storage.")]
  public async Task EnsureAvailableAsyncEntityMetadata_it_should_throw_NotEnoughAvailableStorageException_when_there_is_not_enough_storage()
  {
    _accountSettings.AllocatedBytes = 0;

    EntityMetadata entity = _world.GetMetadata();

    var exception = await Assert.ThrowsAsync<NotEnoughAvailableStorageException>(async () => await _service.EnsureAvailableAsync(entity, _cancellationToken));
    Assert.Equal(_world.OwnerId.ToGuid(), exception.UserId);
    Assert.Equal(_accountSettings.AllocatedBytes, exception.AvailableBytes);
    Assert.Equal(entity.Size, exception.RequiredBytes);
  }

  [Fact(DisplayName = "EnsureAvailableAsync(World): it should throw NotEnoughAvailableStorageException when there is not enough storage.")]
  public async Task EnsureAvailableAsyncWorld_it_should_throw_NotEnoughAvailableStorageException_when_there_is_not_enough_storage()
  {
    _accountSettings.AllocatedBytes = 0;

    var exception = await Assert.ThrowsAsync<NotEnoughAvailableStorageException>(async () => await _service.EnsureAvailableAsync(_world, _cancellationToken));
    Assert.Equal(_world.OwnerId.ToGuid(), exception.UserId);
    Assert.Equal(_accountSettings.AllocatedBytes, exception.AvailableBytes);

    EntityMetadata entity = _world.GetMetadata();
    Assert.Equal(entity.Size, exception.RequiredBytes);
  }

  [Fact(DisplayName = "LoadOrInitializeAsync(EntityMetadata): it should throw InvalidOperationException when the world could not be found.")]
  public async Task LoadOrInitializeAsyncEntityMetadata_it_should_throw_InvalidOperationException_when_the_world_could_not_be_found()
  {
    World world = new(_world.Slug, _world.OwnerId);
    EntityMetadata entity = world.GetMetadata();

    var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () => await _service.EnsureAvailableAsync(entity, _cancellationToken));
    Assert.StartsWith($"The world 'Id={world.Id}' could not be found.", exception.Message);

    exception = await Assert.ThrowsAsync<InvalidOperationException>(async () => await _service.UpdateAsync(entity, _cancellationToken));
    Assert.StartsWith($"The world 'Id={world.Id}' could not be found.", exception.Message);
  }

  [Fact(DisplayName = "UpdateAsync(EntityMetadata): it should update the storage.")]
  public async Task UpdateAsyncEntityMetadata_it_should_update_the_storage()
  {
    EntityMetadata entity = _world.GetMetadata();

    await _service.UpdateAsync(entity, _cancellationToken);

    _storageRepository.Verify(x => x.SaveAsync(It.Is<Storage>(y => y.UserId == _world.OwnerId
      && y.AllocatedBytes == _accountSettings.AllocatedBytes
      && y.UsedBytes == entity.Size), _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "UpdateAsync(World): it should update the storage.")]
  public async Task UpdateAsyncWorld_it_should_update_the_storage()
  {
    await _service.UpdateAsync(_world, _cancellationToken);

    EntityMetadata entity = _world.GetMetadata();
    _storageRepository.Verify(x => x.SaveAsync(It.Is<Storage>(y => y.UserId == _world.OwnerId
      && y.AllocatedBytes == _accountSettings.AllocatedBytes
      && y.UsedBytes == entity.Size), _cancellationToken), Times.Once);
  }
}
