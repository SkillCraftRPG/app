using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Users;
using SkillCraft.Application.Settings;
using SkillCraft.Application.Worlds;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Worlds;
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

  public async Task EnsureCanCommentAsync(Activity activity, EntityMetadata entity, CancellationToken cancellationToken)
  {
    await EnsureCanAsync(Action.Comment, activity, entity, cancellationToken);
  }
  public async Task EnsureCanCommentAsync(Activity activity, WorldModel world, CancellationToken cancellationToken)
  {
    await EnsureCanAsync(Action.Comment, activity, world, cancellationToken);
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

  public async Task EnsureCanPreviewAsync(Activity activity, EntityType entityType, CancellationToken cancellationToken)
  {
    if (entityType == EntityType.World)
    {
      throw new ArgumentOutOfRangeException(nameof(entityType));
    }

    User user = activity.GetUser();
    WorldModel world = activity.GetWorld();
    await EnsureIsOwnerOrHasPermissionAsync(user, world, Action.Preview, entityType, entityId: null, cancellationToken);
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

  public async Task EnsureCanViewAsync(Activity activity, EntityMetadata entity, CancellationToken cancellationToken)
  {
    if (entity.Type == EntityType.World)
    {
      throw new ArgumentException($"The entity type must not be '{EntityType.World}'.", nameof(entity));
    }

    User user = activity.GetUser();
    WorldModel world = activity.GetWorld();
    if (!entity.ResidesIn(world))
    {
      throw new PermissionDeniedException(Action.View, entity.Type, user, world, entity.Id);
    }

    await EnsureIsOwnerOrHasPermissionAsync(user, world, Action.View, entity.Type, entityId: null, cancellationToken);
  }
  public Task EnsureCanViewAsync(Activity activity, WorldModel world, CancellationToken cancellationToken)
  {
    User user = activity.GetUser();
    WorldModel? otherWorld = activity.TryGetWorld();
    if ((otherWorld != null && otherWorld.Id != world.Id) || !user.IsOwner(world) /* && !user.CanPreview(entity) // Member with preview access or world is public? */)
    {
      throw new PermissionDeniedException(Action.View, EntityType.World, user, otherWorld, world.Id);
    }

    return Task.CompletedTask;
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
  private async Task EnsureCanAsync(Action action, Activity activity, WorldModel world, CancellationToken cancellationToken)
  {
    User user = activity.GetUser();
    WorldModel? otherWorld = activity.TryGetWorld();
    Guid worldId = world.Id;
    if (otherWorld != null && otherWorld.Id != worldId)
    {
      throw new PermissionDeniedException(action, EntityType.World, user, otherWorld, worldId);
    }

    await EnsureIsOwnerOrHasPermissionAsync(user, world, action, EntityType.World, world.Id, cancellationToken);
  }

  private async Task EnsureIsOwnerOrHasPermissionAsync(User user, WorldModel world, Action action, EntityType entityType, Guid? entityId, CancellationToken cancellationToken)
  {
    if (!user.IsOwner(world) && !await _permissionQuerier.HasAsync(user, world, action, entityType, entityId, cancellationToken))
    {
      throw new PermissionDeniedException(action, entityType, user, world, entityId);
    }
  }
}
