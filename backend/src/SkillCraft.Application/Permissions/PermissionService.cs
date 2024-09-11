using Logitar.Portal.Contracts.Users;
using SkillCraft.Application.Settings;
using SkillCraft.Application.Worlds;
using SkillCraft.Contracts.Worlds;
using SkillCraft.Domain;

namespace SkillCraft.Application.Permissions;

internal class PermissionService : IPermissionService
{
  private readonly AccountSettings _accountSettings;
  private readonly IWorldQuerier _worldQuerier;

  public PermissionService(AccountSettings accountSettings, IWorldQuerier worldQuerier)
  {
    _accountSettings = accountSettings;
    _worldQuerier = worldQuerier;
  }

  public async Task EnsureCanCreateAsync(Activity activity, EntityType entityType, CancellationToken cancellationToken)
  {
    User user = activity.GetUser();
    if (entityType == EntityType.World)
    {
      int count = await _worldQuerier.CountOwnedAsync(activity.GetUserId(), cancellationToken);
      if (count >= _accountSettings.WorldLimit)
      {
        throw new PermissionDeniedException(Action.Create, entityType, user);
      }
    }
    else
    {
      WorldModel world = activity.GetWorld();
      if (!IsOwner(user, world))
      {
        throw new PermissionDeniedException(Action.Create, entityType, user, world);
      }
    }
  }

  private static bool IsOwner(User user, WorldModel world) => world.Owner.Id == user.Id;
}
