using Logitar.Portal.Contracts.Users;
using SkillCraft.Application.Settings;
using SkillCraft.Application.Worlds;
using SkillCraft.Contracts.Worlds;
using SkillCraft.Domain;
using SkillCraft.Domain.Worlds;

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

  public Task EnsureCanPreviewAsync(Activity activity, WorldModel entity, CancellationToken cancellationToken)
  {
    User user = activity.GetUser();
    WorldModel? world = activity.TryGetWorld();

    if ((world != null && !world.Equals(entity)) || !IsOwner(user, entity))
    {
      throw new PermissionDeniedException(Action.Preview, EntityType.World, user, world, entity.Id);
    }

    return Task.CompletedTask;
  }

  public Task EnsureCanUpdateAsync(Activity activity, EntityMetadata entity, CancellationToken cancellationToken)
  {
    User user = activity.GetUser();
    WorldModel world = activity.GetWorld();

    if (entity.WorldId.ToGuid() != world.Id || !IsOwner(user, world))
    {
      throw new PermissionDeniedException(Action.Update, entity.Key.Type, user, world, entity.Key.Id);
    }

    return Task.CompletedTask;
  }
  public Task EnsureCanUpdateAsync(Activity activity, World world, CancellationToken cancellationToken)
  {
    User user = activity.GetUser();
    WorldModel? model = activity.TryGetWorld();

    if ((model != null && world.Id.ToGuid() != model.Id) || !IsOwner(user, world))
    {
      throw new PermissionDeniedException(Action.Update, EntityType.World, user, model, world.Id.ToGuid());
    }

    return Task.CompletedTask;
  }

  private static bool IsOwner(User user, World world) => world.OwnerId.ToGuid() == user.Id;
  private static bool IsOwner(User user, WorldModel world) => world.Owner.Id == user.Id;
}
