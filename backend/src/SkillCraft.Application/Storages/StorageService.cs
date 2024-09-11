using SkillCraft.Application.Settings;
using SkillCraft.Domain.Storages;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Storages;

internal class StorageService : IStorageService
{
  private readonly Dictionary<WorldId, Storage> _cache = [];

  private readonly AccountSettings _accountSettings;
  private readonly IStorageRepository _storageRepository;
  private readonly IWorldRepository _worldRepository;

  public StorageService(AccountSettings accountSettings, IStorageRepository storageRepository, IWorldRepository worldRepository)
  {
    _accountSettings = accountSettings;
    _storageRepository = storageRepository;
    _worldRepository = worldRepository;
  }

  public async Task EnsureAvailableAsync(EntityMetadata entity, CancellationToken cancellationToken)
  {
    Storage storage = await LoadOrInitializeAsync(entity.WorldId, cancellationToken);

    long previousBytes = storage.GetSize(entity.StorageKey);
    long requiredBytes = entity.Size - previousBytes;
    if (requiredBytes > 0 && requiredBytes > storage.AvailableBytes)
    {
      throw new NotEnoughAvailableStorageException(storage, requiredBytes);
    }
  }

  public async Task UpdateAsync(EntityMetadata entity, CancellationToken cancellationToken)
  {
    Storage storage = await LoadOrInitializeAsync(entity.WorldId, cancellationToken);

    storage.Store(entity.StorageKey, entity.Size, entity.WorldId);

    await _storageRepository.SaveAsync(storage, cancellationToken);
  }

  private async Task<Storage> LoadOrInitializeAsync(WorldId worldId, CancellationToken cancellationToken)
  {
    if (_cache.TryGetValue(worldId, out Storage? storage))
    {
      return storage;
    }

    storage = await _storageRepository.LoadAsync(worldId, cancellationToken);
    if (storage == null)
    {
      World world = await _worldRepository.LoadAsync(worldId, cancellationToken)
        ?? throw new InvalidOperationException($"The world 'Id={worldId}' could not be found.");

      storage = Storage.Initialize(world.OwnerId, _accountSettings.AllocatedBytes);
    }

    _cache[worldId] = storage;

    return storage;
  }
}
