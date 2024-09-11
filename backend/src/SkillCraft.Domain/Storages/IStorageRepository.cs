using SkillCraft.Domain.Worlds;

namespace SkillCraft.Domain.Storages;

public interface IStorageRepository
{
  Task<Storage?> LoadAsync(WorldId worldId, CancellationToken cancellationToken = default);

  Task SaveAsync(Storage storage, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<Storage> storages, CancellationToken cancellationToken = default);
}
