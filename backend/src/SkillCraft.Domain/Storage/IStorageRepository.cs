namespace SkillCraft.Domain.Storage;

public interface IStorageRepository
{
  Task<StorageAggregate?> LoadAsync(Guid userId, CancellationToken cancellationToken = default);
}
