using Logitar.Portal.Contracts.Users;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Permissions;

public interface IPermissionService
{
  Task EnsureCanCreateWorldAsync(User user, CancellationToken cancellationToken = default);
  void EnsureCanDeleteWorld(User user, WorldAggregate world);
  void EnsureCanUpdateWorld(User user, WorldAggregate world);
}
