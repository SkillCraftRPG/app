using SkillCraft.Domain.Worlds;

namespace SkillCraft.Domain.Storage;

public interface IStorageRepository
{
  Task<StorageAggregate?> LoadAsync(WorldId worldId, CancellationToken cancellationToken = default);
}
