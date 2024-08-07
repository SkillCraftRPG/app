using SkillCraft.Domain;

namespace SkillCraft.Application.Storage;

public interface IStorageService
{
  Task EnsureAvailableAsync(IStoredEntity entity, CancellationToken cancellationToken = default);

  Task DeleteAsync(IStoredEntity entity, CancellationToken cancellationToken = default);
  Task UpdateAsync(IStoredEntity entity, CancellationToken cancellationToken = default);
}
