using Moq;
using SkillCraft.Application.Settings;
using SkillCraft.Domain.Storage;

namespace SkillCraft.Application.Storage;

[Trait(Traits.Category, Categories.Unit)]
public class StorageServiceTests
{
  private readonly CancellationToken _cancellationToken = default;
  private readonly Random _random = new();

  private readonly AccountSettings _accountSettings = new();
  private readonly Mock<IStorageRepository> _storageRepository = new();

  private readonly StorageService _service;

  private readonly UserMock _user = new();

  public StorageServiceTests()
  {
    _service = new(_accountSettings, _storageRepository.Object);
  }

  [Theory(DisplayName = "EnsureAvailableAsync: it should not check storage when zero or negative bytes are required.")]
  [InlineData(0)]
  [InlineData(-100)]
  public async Task EnsureAvailableAsync_it_should_not_check_storage_when_zero_or_negative_bytes_are_required(int requiredBytes)
  {
    Assert.True(requiredBytes <= 0);

    StoredEntityMock entity = new(requiredBytes);
    await _service.EnsureAvailableAsync(_user, entity, 0, _cancellationToken);

    _storageRepository.VerifyNoOtherCalls();
  }

  [Theory(DisplayName = "EnsureAvailableAsync: it should succeed when there is enough available storage.")]
  [InlineData(false)]
  [InlineData(true)]
  public async Task EnsureAvailableAsync_it_should_succeed_when_there_is_enough_available_storage(bool summaryExists)
  {
    StoredEntityMock entity = new(_random.Next(1, byte.MaxValue + 1));
    int previousSize = entity.Size / 2;
    _accountSettings.AllocatedBytes = entity.Size * 3 / 4;

    if (summaryExists)
    {
      StorageAggregate storage = StorageAggregate.Initialize(_user.Id, _accountSettings.AllocatedBytes);
      _storageRepository.Setup(x => x.LoadAsync(_user.Id, _cancellationToken)).ReturnsAsync(storage);
    }

    await _service.EnsureAvailableAsync(_user, entity, previousSize, _cancellationToken);
  }

  [Theory(DisplayName = "EnsureAvailableAsync: it should throw NotEnoughAvailableStorageException when there is not enough storage.")]
  [InlineData(false)]
  [InlineData(true)]
  public async Task EnsureAvailableAsync_it_should_throw_NotEnoughAvailableStorageException_when_there_is_not_enough_storage(bool summaryExists)
  {
    StoredEntityMock entity = new(_random.Next(1, byte.MaxValue + 1));
    int previousSize = entity.Size / 2;
    _accountSettings.AllocatedBytes = entity.Size / 4;

    if (summaryExists)
    {
      StorageAggregate storage = StorageAggregate.Initialize(_user.Id, _accountSettings.AllocatedBytes);
      _storageRepository.Setup(x => x.LoadAsync(_user.Id, _cancellationToken)).ReturnsAsync(storage);
    }

    var exception = await Assert.ThrowsAsync<NotEnoughAvailableStorageException>(async () => await _service.EnsureAvailableAsync(_user, entity, previousSize, _cancellationToken));
    Assert.Equal(_user.Id, exception.UserId);
    Assert.Equal(_accountSettings.AllocatedBytes, exception.AvailableBytes);
    Assert.Equal(entity.Size - previousSize, exception.RequiredBytes);
  }
}
