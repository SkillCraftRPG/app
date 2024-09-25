using Logitar.Portal.Contracts.Users;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Worlds;

namespace SkillCraft.Application.Permissions;

public interface IPermissionQuerier
{
  Task<bool> HasAsync(User user, WorldModel world, Action action, EntityType? entityType, Guid? entityId, CancellationToken cancellationToken);
}
