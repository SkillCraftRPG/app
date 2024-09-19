using Logitar.Portal.Contracts.Users;
using SkillCraft.Contracts.Worlds;

namespace SkillCraft.Application.Permissions;

internal static class PermissionExtensions
{
  public static bool IsOwner(this User user, WorldModel world) => world.Owner.Id == user.Id;
  public static bool ResidesIn(this EntityMetadata entity, WorldModel world) => world.Id == entity.WorldId.ToGuid();
}
