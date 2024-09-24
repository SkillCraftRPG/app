using SkillCraft.Application.Settings;
using SkillCraft.Application.Worlds;
using SkillCraft.Contracts.Worlds;
using SkillCraft.Domain;
using SkillCraft.Domain.Storages;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Storages;

internal class StorageService : IStorageService
{
  private readonly Dictionary<WorldId, Storage> _cache = [];

  private readonly AccountSettings _accountSettings;
  private readonly IStorageRepository _storageRepository;
  private readonly IWorldQuerier _worldQuerier;

  public StorageService(AccountSettings accountSettings, IStorageRepository storageRepository, IWorldQuerier worldQuerier)
  {
    _accountSettings = accountSettings;
    _storageRepository = storageRepository;
    _worldQuerier = worldQuerier;
  }

  public async Task EnsureAvailableAsync(EntityMetadata entity, CancellationToken cancellationToken)
  {
    Storage storage = await LoadOrInitializeAsync(entity.WorldId, cancellationToken);

    EnsureAvailable(storage, entity);
  }
  public async Task EnsureAvailableAsync(World world, CancellationToken cancellationToken)
  {
    Storage storage = await LoadOrInitializeAsync(world, cancellationToken);

    EntityMetadata entity = world.GetMetadata();
    EnsureAvailable(storage, entity);
  }
  private static void EnsureAvailable(Storage storage, EntityMetadata entity)
  {
    long previousBytes = storage.GetSize(entity.Key);
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

    EntityMetadata entity = world.GetMetadata();
    await UpdateAsync(storage, entity, cancellationToken);
  }
  private async Task UpdateAsync(Storage storage, EntityMetadata entity, CancellationToken cancellationToken)
  {
    storage.Store(entity.Key, entity.Size, entity.WorldId);

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
      WorldModel world = await _worldQuerier.ReadAsync(worldId, cancellationToken)
        ?? throw new InvalidOperationException($"The world 'Id={worldId}' could not be found.");

      UserId ownerId = new(world.Owner.Id);
      storage = Storage.Initialize(ownerId, _accountSettings.AllocatedBytes);
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
