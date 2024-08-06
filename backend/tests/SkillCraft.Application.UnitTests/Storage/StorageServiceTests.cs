using Logitar.EventSourcing;
using Moq;
using SkillCraft.Application.Settings;
using SkillCraft.Domain;
using SkillCraft.Domain.Storage;
using SkillCraft.Domain.Storage.Events;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Storage;

[Trait(Traits.Category, Categories.Unit)]
public class StorageServiceTests
{
  private readonly CancellationToken _cancellationToken = default;
  private readonly Random _random = new();

  private readonly AccountSettings _accountSettings = new()
  {
    AllocatedBytes = 1024
  };
  private readonly Mock<IStorageRepository> _storageRepository = new();
  private readonly Mock<IWorldRepository> _worldRepository = new();

  private readonly StorageService _service;

  private readonly WorldAggregate _world;

  public StorageServiceTests()
  {
    _service = new(_accountSettings, _storageRepository.Object, _worldRepository.Object);

    _world = new(new SlugUnit("new-world"), ActorId.NewId());
    _worldRepository.Setup(x => x.LoadAsync(_world.Id, _cancellationToken)).ReturnsAsync(_world);
  }

  [Fact(DisplayName = "EnsureAvailableAsync: it should cache a new aggregate when there is enough storage.")]
  public async Task EnsureAvailableAsync_it_should_cache_a_new_aggregate_when_there_is_enough_storage()
  {
    StoredEntityMock entity = new(_world.Id, EntityType.World, _world.Id.ToGuid(), _random.Next(1, (int)_accountSettings.AllocatedBytes / 2));

    await _service.EnsureAvailableAsync(entity, _cancellationToken);

    AssertCached();
  }

  [Fact(DisplayName = "EnsureAvailableAsync: it should cache the existing aggregate when there is enough storage.")]
  public async Task EnsureAvailableAsync_it_should_cache_the_existing_aggregate_when_there_is_enough_storage()
  {
    StorageAggregate storage = StorageAggregate.Initialize(_world.OwnerId, _accountSettings.AllocatedBytes);
    _storageRepository.Setup(x => x.LoadAsync(_world.Id, _cancellationToken)).ReturnsAsync(storage);

    StoredEntityMock previousEntity = new(_world.Id, EntityType.World, _world.Id.ToGuid(), _random.Next(1, (int)storage.AllocatedBytes / 2));
    storage.Store(previousEntity);

    StoredEntityMock currentEntity = new(previousEntity.WorldId, previousEntity.EntityType, previousEntity.EntityId, previousEntity.Size * 3 / 2);
    await _service.EnsureAvailableAsync(currentEntity, _cancellationToken);

    AssertCached(storage);

    _worldRepository.Verify(x => x.LoadAsync(It.IsAny<WorldId>(), It.IsAny<CancellationToken>()), Times.Never);
  }

  [Fact(DisplayName = "EnsureAvailableAsync: it should throw InvalidOperationException when the world could not be found.")]
  public async Task EnsureAvailableAsync_it_should_throw_InvalidOperationException_when_the_world_could_not_be_found()
  {
    StoredEntityMock entity = new(new WorldId(Guid.Empty), EntityType.World, Guid.Empty, _random.Next(1, (int)_accountSettings.AllocatedBytes));
    var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () => await _service.UpdateAsync(entity, _cancellationToken));
    Assert.Equal($"The world aggregate 'Id={entity.WorldId}' could not be found.", exception.Message);
  }

  [Fact(DisplayName = "EnsureAvailableAsync: it should throw NotEnoughAvailableStorageException when there is not enough storage.")]
  public async Task EnsureAvailableAsync_it_should_throw_NotEnoughAvailableStorageException_when_there_is_not_enough_storage()
  {
    StorageAggregate storage = StorageAggregate.Initialize(_world.OwnerId, _accountSettings.AllocatedBytes);
    _storageRepository.Setup(x => x.LoadAsync(_world.Id, _cancellationToken)).ReturnsAsync(storage);

    StoredEntityMock previousEntity = new(_world.Id, EntityType.World, _world.Id.ToGuid(), _random.Next(1, (int)storage.AllocatedBytes));
    storage.Store(previousEntity);

    StoredEntityMock currentEntity = new(previousEntity.WorldId, previousEntity.EntityType, previousEntity.EntityId, (int)storage.AllocatedBytes + 1);
    var exception = await Assert.ThrowsAsync<NotEnoughAvailableStorageException>(async () => await _service.EnsureAvailableAsync(currentEntity, _cancellationToken));
    Assert.Equal(storage.UserId, exception.UserId);
    Assert.Equal(storage.AvailableBytes, exception.AvailableBytes);
    Assert.Equal(currentEntity.Size - previousEntity.Size, exception.RequiredBytes);
  }

  [Fact(DisplayName = "UpdateAsync: it should reuse a cached storage.")]
  public async Task UpdateAsync_it_should_reuse_a_cached_storage()
  {
    StorageAggregate storage = StorageAggregate.Initialize(_world.OwnerId, _accountSettings.AllocatedBytes);

    Dictionary<WorldId, StorageAggregate>? cache = _service.GetType()
      .GetField("_cache", BindingFlags.Instance | BindingFlags.NonPublic)
      ?.GetValue(_service) as Dictionary<WorldId, StorageAggregate>;
    Assert.NotNull(cache);
    cache[_world.Id] = storage;

    StoredEntityMock entity = new(_world.Id, EntityType.World, _world.Id.ToGuid(), _random.Next(1, (int)storage.AllocatedBytes));
    await _service.UpdateAsync(entity, _cancellationToken);

    _storageRepository.Verify(x => x.LoadAsync(It.IsAny<WorldId>(), It.IsAny<CancellationToken>()), Times.Never);
    _storageRepository.Verify(x => x.SaveAsync(storage, _cancellationToken), Times.Once);
    _worldRepository.Verify(x => x.LoadAsync(It.IsAny<WorldId>(), It.IsAny<CancellationToken>()), Times.Never);

    AssertCached(storage);
  }

  [Fact(DisplayName = "UpdateAsync: it should store an entity in a new storage when it did not exist.")]
  public async Task UpdateAsync_it_should_store_an_entity_in_a_new_storage_when_it_did_not_exist()
  {
    StoredEntityMock entity = new(_world.Id, EntityType.World, _world.Id.ToGuid(), _random.Next(1, (int)_accountSettings.AllocatedBytes));
    await _service.UpdateAsync(entity, _cancellationToken);

    Func<DomainEvent, bool> isEntityStoredEvent = change => change is EntityStoredEvent e && e.WorldId == entity.WorldId
      && e.EntityType == entity.EntityType && e.EntityId == entity.EntityId && e.Size == entity.Size && e.UsedBytes == entity.Size;
    _storageRepository.Verify(x => x.SaveAsync(It.Is<StorageAggregate>(y => y.UserId == _world.OwnerId
      && y.AllocatedBytes == _accountSettings.AllocatedBytes
      && y.Changes.Count == 2 && y.Changes.Any(isEntityStoredEvent)), _cancellationToken), Times.Once);

    AssertCached();
  }

  [Fact(DisplayName = "UpdateAsync: it should store an entity in an existing storage when it did exist.")]
  public async Task UpdateAsync_it_should_store_an_entity_in_an_existing_storage_when_it_did_exist()
  {
    StorageAggregate storage = StorageAggregate.Initialize(_world.OwnerId, _accountSettings.AllocatedBytes);
    _storageRepository.Setup(x => x.LoadAsync(_world.Id, _cancellationToken)).ReturnsAsync(storage);

    StoredEntityMock previousEntity = new(_world.Id, EntityType.World, _world.Id.ToGuid(), _random.Next(1, (int)_accountSettings.AllocatedBytes / 2));
    storage.Store(previousEntity);

    StoredEntityMock currentEntity = new(previousEntity.WorldId, previousEntity.EntityType, previousEntity.EntityId, previousEntity.Size * 3 / 2);
    await _service.UpdateAsync(currentEntity, _cancellationToken);

    _worldRepository.Verify(x => x.LoadAsync(It.IsAny<WorldId>(), It.IsAny<CancellationToken>()), Times.Never);

    Func<DomainEvent, bool> isEntityStoredEvent = change => change is EntityStoredEvent e && e.WorldId == currentEntity.WorldId
      && e.EntityType == currentEntity.EntityType && e.EntityId == currentEntity.EntityId && e.Size == currentEntity.Size && e.UsedBytes == currentEntity.Size;
    _storageRepository.Verify(x => x.SaveAsync(It.Is<StorageAggregate>(y => y.UserId == _world.OwnerId
      && y.AllocatedBytes == _accountSettings.AllocatedBytes
      && y.Changes.Count == 3 && y.Changes.Any(isEntityStoredEvent)), _cancellationToken), Times.Once);

    AssertCached(storage);
  }

  [Fact(DisplayName = "UpdateAsync: it should throw InvalidOperationException when the world could not be found.")]
  public async Task UpdateAsync_it_should_throw_InvalidOperationException_when_the_world_could_not_be_found()
  {
    StoredEntityMock entity = new(new WorldId(Guid.Empty), EntityType.World, Guid.Empty, _random.Next(1, (int)_accountSettings.AllocatedBytes));
    var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () => await _service.UpdateAsync(entity, _cancellationToken));
    Assert.Equal($"The world aggregate 'Id={entity.WorldId}' could not be found.", exception.Message);
  }

  private void AssertCached(StorageAggregate? storage = null)
  {
    Dictionary<WorldId, StorageAggregate>? cache = _service.GetType()
      .GetField("_cache", BindingFlags.Instance | BindingFlags.NonPublic)
      ?.GetValue(_service) as Dictionary<WorldId, StorageAggregate>;
    Assert.NotNull(cache);
    KeyValuePair<WorldId, StorageAggregate> cached = Assert.Single(cache);
    Assert.Equal(_world.Id, cached.Key);

    if (storage == null)
    {
      storage = cached.Value;
      Assert.Equal(_world.OwnerId, storage.UserId);
      Assert.Equal(_accountSettings.AllocatedBytes, storage.AllocatedBytes);
    }
    else
    {
      Assert.Same(storage, cached.Value);
    }
  }
}
