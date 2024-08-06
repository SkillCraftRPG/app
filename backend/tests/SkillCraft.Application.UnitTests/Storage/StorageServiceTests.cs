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

  private readonly AccountSettings _accountSettings = new();
  private readonly Mock<IStorageRepository> _storageRepository = new();
  private readonly Mock<IWorldRepository> _worldRepository = new();

  private readonly StorageService _service;

  private readonly WorldAggregate _world;

  public StorageServiceTests()
  {
    _service = new(_accountSettings, _storageRepository.Object, _worldRepository.Object);

    _world = new(new SlugUnit("new-world"), ActorId.NewId());
  }

  [Theory(DisplayName = "EnsureAvailableAsync: it should not check storage when zero or negative bytes are required.")]
  [InlineData(0)]
  [InlineData(-100)]
  public async Task EnsureAvailableAsync_it_should_not_check_storage_when_zero_or_negative_bytes_are_required(int requiredBytes)
  {
    Assert.True(requiredBytes <= 0);

    StoredEntityMock entity = new(_world.Id, _world.EntityType, _world.EntityId, requiredBytes);
    await _service.EnsureAvailableAsync(entity, 0, _cancellationToken);

    _storageRepository.VerifyNoOtherCalls();
  }

  [Theory(DisplayName = "EnsureAvailableAsync: it should succeed when there is enough available storage.")]
  [InlineData(false)]
  [InlineData(true)]
  public async Task EnsureAvailableAsync_it_should_succeed_when_there_is_enough_available_storage(bool summaryExists)
  {
    StoredEntityMock entity = new(_world.Id, _world.EntityType, _world.EntityId, _random.Next(1, byte.MaxValue + 1));
    int previousSize = entity.Size / 2;
    _accountSettings.AllocatedBytes = entity.Size * 3 / 4;

    if (summaryExists)
    {
      StorageAggregate storage = StorageAggregate.Initialize(_world.OwnerId.ToGuid(), _accountSettings.AllocatedBytes);
      _storageRepository.Setup(x => x.LoadAsync(_world.Id, _cancellationToken)).ReturnsAsync(storage);
    }

    await _service.EnsureAvailableAsync(entity, previousSize, _cancellationToken);
  }

  [Theory(DisplayName = "EnsureAvailableAsync: it should throw NotEnoughAvailableStorageException when there is not enough storage.")]
  [InlineData(false)]
  [InlineData(true)]
  public async Task EnsureAvailableAsync_it_should_throw_NotEnoughAvailableStorageException_when_there_is_not_enough_storage(bool summaryExists)
  {
    StoredEntityMock entity = new(_world.Id, _world.EntityType, _world.EntityId, _random.Next(1, byte.MaxValue + 1));
    int previousSize = entity.Size / 2;
    _accountSettings.AllocatedBytes = entity.Size / 4;

    if (summaryExists)
    {
      StorageAggregate storage = StorageAggregate.Initialize(_world.OwnerId.ToGuid(), _accountSettings.AllocatedBytes);
      _storageRepository.Setup(x => x.LoadAsync(_world.Id, _cancellationToken)).ReturnsAsync(storage);
    }
    else
    {
      _worldRepository.Setup(x => x.LoadAsync(_world.Id, _cancellationToken)).ReturnsAsync(_world);
    }

    var exception = await Assert.ThrowsAsync<NotEnoughAvailableStorageException>(async () => await _service.EnsureAvailableAsync(entity, previousSize, _cancellationToken));
    Assert.Equal(_world.OwnerId.ToGuid(), exception.UserId);
    Assert.Equal(_accountSettings.AllocatedBytes, exception.AvailableBytes);
    Assert.Equal(entity.Size - previousSize, exception.RequiredBytes);
  }

  [Fact(DisplayName = "UpdateAsync: it should initialize the storage when it does not exist.")]
  public async Task UpdateAsync_it_should_initialize_the_storage_when_it_does_not_exist()
  {
    _accountSettings.AllocatedBytes = 1024;
    _worldRepository.Setup(x => x.LoadAsync(_world.Id, _cancellationToken)).ReturnsAsync(_world);

    StoredEntityMock entity = new(_world.Id, EntityType.World, _world.Id.ToGuid(), _world.Size);
    await _service.UpdateAsync(entity, _cancellationToken);

    Func<DomainEvent, bool> isStorageInitializedEvent = change => change is StorageInitializedEvent e && e.UserId == _world.OwnerId.ToGuid() && e.AllocatedBytes == _accountSettings.AllocatedBytes;
    Func<DomainEvent, bool> isEntityStoredEvent = change => change is EntityStoredEvent e && e.WorldId == _world.Id && e.EntityType == entity.EntityType && e.EntityId == entity.EntityId && e.Size == entity.Size && e.UsedBytes == entity.Size; ;
    _storageRepository.Verify(x => x.SaveAsync(It.Is<StorageAggregate>(y => y.Changes.Count == 2
      && y.Changes.Any(isStorageInitializedEvent)
      && y.Changes.Any(isEntityStoredEvent)
    ), _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "UpdateAsync: it should update the storage when it exists.")]
  public async Task UpdateAsync_it_should_update_the_storage_when_it_exists()
  {
    StorageAggregate storage = StorageAggregate.Initialize(_world.OwnerId.ToGuid(), allocatedBytes: 1024);
    storage.ClearChanges();
    _storageRepository.Setup(x => x.LoadAsync(_world.Id, _cancellationToken)).ReturnsAsync(storage);

    StoredEntityMock entity = new(_world.Id, EntityType.World, _world.Id.ToGuid(), _world.Size);
    await _service.UpdateAsync(entity, _cancellationToken);

    _worldRepository.Verify(x => x.LoadAsync(It.IsAny<WorldId>(), It.IsAny<CancellationToken>()), Times.Never);

    Func<DomainEvent, bool> isEntityStoredEvent = change => change is EntityStoredEvent e && e.WorldId == _world.Id && e.EntityType == entity.EntityType && e.EntityId == entity.EntityId && e.Size == entity.Size && e.UsedBytes == entity.Size; ;
    _storageRepository.Verify(x => x.SaveAsync(It.Is<StorageAggregate>(y => y.Changes.Count == 1 && y.Changes.Any(isEntityStoredEvent)), _cancellationToken), Times.Once);
  }
}
