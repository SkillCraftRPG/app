using Logitar.Portal.Contracts.Users;
using SkillCraft.Domain;

namespace SkillCraft.Application.Storage;

public interface IStorageService
{
  Task EnsureAvailableAsync(User user, IStoredEntity entity, CancellationToken cancellationToken = default);
  Task EnsureAvailableAsync(User user, IStoredEntity entity, int previousSize, CancellationToken cancellationToken = default);
}
