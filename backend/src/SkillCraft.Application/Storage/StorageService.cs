using SkillCraft.Application.Settings;
using SkillCraft.Domain;
using SkillCraft.Domain.Storage;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Storage;

internal class StorageService : IStorageService
{
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
    await EnsureAvailableAsync(entity, previousSize: 0, cancellationToken);
  }
  public async Task EnsureAvailableAsync(IStoredEntity entity, int previousSize, CancellationToken cancellationToken)
  {
    int requiredBytes = entity.Size - previousSize;
    if (requiredBytes > 0)
    {
      StorageAggregate? storage = await _storageRepository.LoadAsync(entity.WorldId, cancellationToken);
      long availableBytes = storage?.AvailableBytes ?? _accountSettings.AllocatedBytes;
      if (requiredBytes > availableBytes)
      {
        Guid? userId = storage?.UserId;
        if (!userId.HasValue)
        {
          WorldAggregate world = await _worldRepository.LoadAsync(entity.WorldId, cancellationToken)
            ?? throw new InvalidOperationException($"The world aggregate 'Id={entity.WorldId}' could not be found.");
          userId = world.OwnerId.ToGuid();
        }
        throw new NotEnoughAvailableStorageException(userId.Value, availableBytes, requiredBytes);
      }
    }
  }
}
