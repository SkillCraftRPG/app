using SkillCraft.Domain;

namespace SkillCraft.Application.Storage;

public interface IStorageService
{
  Task EnsureAvailableAsync(IStoredEntity entity, CancellationToken cancellationToken = default);
  Task EnsureAvailableAsync(IStoredEntity entity, int previousSize, CancellationToken cancellationToken = default);
}
