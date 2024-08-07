using SkillCraft.Application.Settings;
using SkillCraft.Domain;
using SkillCraft.Domain.Storage;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Storage;

internal class StorageService : IStorageService
{
  private readonly Dictionary<WorldId, StorageAggregate> _cache = [];

  private readonly AccountSettings _accountSettings;
  private readonly IStorageRepository _storageRepository;
  private readonly IWorldRepository _worldRepository;

  public StorageService(AccountSettings accountSettings, IStorageRepository storageRepository, IWorldRepository worldRepository)
  {
    _accountSettings = accountSettings;
    _storageRepository = storageRepository;
    _worldRepository = worldRepository;
  }

  public async Task EnsureAvailableAsync(IStoredEntity entity, CancellationToken cancellationToken)
  {
    StorageAggregate storage = await LoadOrInitializeAsync(entity.WorldId, cancellationToken);

    long previousBytes = storage.GetSize(entity);
    long requiredBytes = entity.Size - previousBytes;
    if (requiredBytes > 0 && requiredBytes > storage.AvailableBytes)
    {
      throw new NotEnoughAvailableStorageException(storage, requiredBytes);
    }

    _cache[entity.WorldId] = storage;
  }

  public async Task DeleteAsync(IStoredEntity entity, CancellationToken cancellationToken)
  {
    StorageAggregate? storage = await _storageRepository.LoadAsync(entity.WorldId, cancellationToken);
    if (storage != null)
    {
      // TODO(fpion): implement

      await _storageRepository.SaveAsync(storage, cancellationToken);
    }
  }

  public async Task UpdateAsync(IStoredEntity entity, CancellationToken cancellationToken)
  {
    if (!_cache.TryGetValue(entity.WorldId, out StorageAggregate? storage))
    {
      storage = await LoadOrInitializeAsync(entity.WorldId, cancellationToken);
    }

    storage.Store(entity);

    await _storageRepository.SaveAsync(storage, cancellationToken);

    _cache[entity.WorldId] = storage;
  }

  private async Task<StorageAggregate> LoadOrInitializeAsync(WorldId worldId, CancellationToken cancellationToken)
  {
    StorageAggregate? storage = await _storageRepository.LoadAsync(worldId, cancellationToken);
    if (storage == null)
    {
      WorldAggregate world = await _worldRepository.LoadAsync(worldId, cancellationToken)
        ?? throw new InvalidOperationException($"The world aggregate 'Id={worldId}' could not be found.");

      storage = StorageAggregate.Initialize(world.OwnerId, _accountSettings.AllocatedBytes);
    }

    return storage;
  }
}
