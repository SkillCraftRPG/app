using Logitar.Portal.Contracts.Users;
using SkillCraft.Contracts.Worlds;
using SkillCraft.Domain;

namespace SkillCraft.Application.Permissions;

public interface IPermissionQuerier
{
  Task<bool> HasAsync(User user, WorldModel world, Action action, EntityType? entityType, Guid? entityId, CancellationToken cancellationToken);
}
