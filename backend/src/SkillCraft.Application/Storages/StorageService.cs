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

    EnsureAvailable(storage, entity);
  }
  public async Task EnsureAvailableAsync(World world, CancellationToken cancellationToken)
  {
    Storage storage = await LoadOrInitializeAsync(world, cancellationToken);

    EntityMetadata entity = EntityMetadata.From(world);
    EnsureAvailable(storage, entity);
  }
  private static void EnsureAvailable(Storage storage, EntityMetadata entity)
  {
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

    await UpdateAsync(storage, entity, cancellationToken);
  }
  public async Task UpdateAsync(World world, CancellationToken cancellationToken)
  {
    Storage storage = await LoadOrInitializeAsync(world, cancellationToken);

    EntityMetadata entity = EntityMetadata.From(world);
    await UpdateAsync(storage, entity, cancellationToken);
  }
  private async Task UpdateAsync(Storage storage, EntityMetadata entity, CancellationToken cancellationToken)
  {
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
  private async Task<Storage> LoadOrInitializeAsync(World world, CancellationToken cancellationToken)
  {
    if (_cache.TryGetValue(world.Id, out Storage? storage))
    {
      return storage;
    }

    storage = await _storageRepository.LoadAsync(world.Id, cancellationToken)
      ?? Storage.Initialize(world.OwnerId, _accountSettings.AllocatedBytes);

    _cache[world.Id] = storage;

    return storage;
  }
}
