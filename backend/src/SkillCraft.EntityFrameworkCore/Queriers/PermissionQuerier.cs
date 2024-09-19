using Logitar.Portal.Contracts.Users;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts.Worlds;
using SkillCraft.Domain;
using Action = SkillCraft.Application.Permissions.Action;

namespace SkillCraft.EntityFrameworkCore.Queriers;

internal class PermissionQuerier : IPermissionQuerier
{
  public Task<bool> HasAsync(User user, WorldModel world, Action action, EntityType? entityType, Guid? entityId, CancellationToken cancellationToken)
  {
    return Task.FromResult(false);
  }
}

// PK
// World, WorldId(int), WorldId(Guid)
// Action
// EntityType?
// EntityId(Guid?)
// UserId(?) => if null, all members of the world
// IsPublic(bool) for Viewing published documents
