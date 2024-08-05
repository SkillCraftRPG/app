using Logitar.Portal.Contracts.Users;
using SkillCraft.Application.Settings;
using SkillCraft.Domain;
using SkillCraft.Domain.Storage;

namespace SkillCraft.Application.Storage;

internal class StorageService : IStorageService
{
  private readonly AccountSettings _accountSettings;
  private readonly IStorageRepository _storageRepository;

  public StorageService(AccountSettings accountSettings, IStorageRepository storageRepository)
  {
    _accountSettings = accountSettings;
    _storageRepository = storageRepository;
  }

  public async Task EnsureAvailableAsync(User user, IStoredEntity entity, CancellationToken cancellationToken)
  {
    await EnsureAvailableAsync(user, entity, previousSize: 0, cancellationToken);
  }
  public async Task EnsureAvailableAsync(User user, IStoredEntity entity, int previousSize, CancellationToken cancellationToken)
  {
    int requiredBytes = entity.Size - previousSize;
    if (requiredBytes > 0)
    {
      StorageAggregate? storage = await _storageRepository.LoadAsync(user.Id, cancellationToken);
      long availableBytes = storage?.AvailableBytes ?? _accountSettings.AllocatedBytes;
      if (requiredBytes > availableBytes)
      {
        throw new NotEnoughAvailableStorageException(user, availableBytes, requiredBytes);
      }
    }
  }
}
