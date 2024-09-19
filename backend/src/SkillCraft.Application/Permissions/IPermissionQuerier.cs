using Logitar.Portal.Contracts.Users;
using SkillCraft.Contracts.Worlds;
using SkillCraft.Domain;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Permissions;

public interface IPermissionQuerier
{
  Task<bool> HasAsync(User user, Action action, World world, CancellationToken cancellationToken);
  Task<bool> HasAsync(User user, WorldModel world, Action action, EntityType? entityType, Guid? entityId, CancellationToken cancellationToken);
}
