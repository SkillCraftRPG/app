using Logitar.Portal.Contracts.Actors;
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
  private readonly IPermissionQuerier _permissionQuerier;
  private readonly IWorldQuerier _worldQuerier;

  public PermissionService(AccountSettings accountSettings, IPermissionQuerier permissionQuerier, IWorldQuerier worldQuerier)
  {
    _accountSettings = accountSettings;
    _permissionQuerier = permissionQuerier;
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
        throw new PermissionDeniedException(Action.Create, EntityType.World, user);
      }
    }
    else
    {
      WorldModel world = activity.GetWorld();
      await EnsureIsOwnerOrHasPermissionAsync(user, world, Action.Create, entityType, entityId: null, cancellationToken);
    }
  }

  public async Task EnsureCanPreviewAsync(Activity activity, EntityMetadata entity, CancellationToken cancellationToken)
  {
    if (entity.Type == EntityType.World)
    {
      throw new ArgumentException($"The entity type must not be '{EntityType.World}'.", nameof(entity));
    }

    User user = activity.GetUser();
    WorldModel world = activity.GetWorld();
    if (!entity.ResidesIn(world))
    {
      throw new PermissionDeniedException(Action.Preview, entity.Type, user, world, entity.Id);
    }

    await EnsureIsOwnerOrHasPermissionAsync(user, world, Action.Preview, entity.Type, entityId: null, cancellationToken);
  }
  public async Task EnsureCanPreviewAsync(Activity activity, EntityType entityType, CancellationToken cancellationToken)
  {
    if (entityType != EntityType.World)
    {
      User user = activity.GetUser();
      WorldModel world = activity.GetWorld();
      await EnsureIsOwnerOrHasPermissionAsync(user, world, Action.Preview, entityType, entityId: null, cancellationToken);
    }
  }
  public Task EnsureCanPreviewAsync(Activity activity, WorldModel world, CancellationToken cancellationToken)
  {
    User user = activity.GetUser();
    WorldModel? otherWorld = activity.TryGetWorld();
    if ((otherWorld != null && otherWorld.Id != world.Id) || !user.IsOwner(world) /* && !user.CanPreview(entity) // Member with preview access */)
    {
      throw new PermissionDeniedException(Action.Preview, EntityType.World, user, otherWorld, world.Id);
    }

    return Task.CompletedTask;
  }

  public async Task EnsureCanUpdateAsync(Activity activity, EntityMetadata entity, CancellationToken cancellationToken)
  {
    await EnsureCanAsync(Action.Update, activity, entity, cancellationToken);
  }
  public async Task EnsureCanUpdateAsync(Activity activity, World world, CancellationToken cancellationToken)
  {
    await EnsureCanAsync(Action.Update, activity, world, cancellationToken);
  }

  private async Task EnsureCanAsync(Action action, Activity activity, EntityMetadata entity, CancellationToken cancellationToken)
  {
    if (entity.Type == EntityType.World)
    {
      throw new ArgumentException($"The entity type must not be '{EntityType.World}'.", nameof(entity));
    }

    User user = activity.GetUser();
    WorldModel world = activity.GetWorld();
    if (!entity.ResidesIn(world))
    {
      throw new PermissionDeniedException(action, entity.Type, user, world, entity.Id);
    }

    await EnsureIsOwnerOrHasPermissionAsync(user, world, action, entity.Type, entity.Id, cancellationToken);
  }
  private async Task EnsureCanAsync(Action action, Activity activity, World world, CancellationToken cancellationToken)
  {
    User user = activity.GetUser();
    WorldModel? otherWorld = activity.TryGetWorld();
    Guid worldId = world.Id.ToGuid();
    if (otherWorld != null && otherWorld.Id != worldId)
    {
      throw new PermissionDeniedException(action, EntityType.World, user, otherWorld, worldId);
    }

    await EnsureIsOwnerOrHasPermissionAsync(user, world, action, cancellationToken);
  }

  private async Task EnsureIsOwnerOrHasPermissionAsync(User user, World world, Action action, CancellationToken cancellationToken)
  {
    WorldModel model = new()
    {
      Id = world.Id.ToGuid(),
      Owner = new Actor
      {
        Id = world.OwnerId.ToGuid()
      }
    };
    await EnsureIsOwnerOrHasPermissionAsync(user, model, action, EntityType.World, model.Id, cancellationToken);
  }
  private async Task EnsureIsOwnerOrHasPermissionAsync(User user, WorldModel world, Action action, EntityType entityType, Guid? entityId, CancellationToken cancellationToken)
  {
    if (!user.IsOwner(world) && !await _permissionQuerier.HasAsync(user, world, action, entityType, entityId, cancellationToken))
    {
      throw new PermissionDeniedException(action, entityType, user, world, entityId);
    }
  }
}
