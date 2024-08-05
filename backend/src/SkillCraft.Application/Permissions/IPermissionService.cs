using Logitar.Portal.Contracts.Users;

namespace SkillCraft.Application.Permissions;

public interface IPermissionService
{
  Task EnsureCanCreateWorldAsync(User user, CancellationToken cancellationToken = default);
}
