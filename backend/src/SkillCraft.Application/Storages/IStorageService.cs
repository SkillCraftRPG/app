using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Storages;

public interface IStorageService
{
  Task EnsureAvailableAsync(EntityMetadata entity, CancellationToken cancellationToken = default);
  Task EnsureAvailableAsync(World world, CancellationToken cancellationToken = default);

  Task UpdateAsync(EntityMetadata entity, CancellationToken cancellationToken = default);
  Task UpdateAsync(World world, CancellationToken cancellationToken = default);
}
