using Logitar.Portal.Contracts.Users;
using SkillCraft.Application.Settings;
using SkillCraft.Application.Worlds;
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

  public async Task EnsureCanCreateWorldAsync(User user, CancellationToken cancellationToken)
  {
    int count = await _worldQuerier.CountOwnedAsync(user, cancellationToken);
    if (count >= _accountSettings.WorldLimit)
    {
      throw new PermissionDeniedException(Action.Create, EntityType.World, user);
    }
  }

  public void EnsureCanDeleteWorld(User user, WorldAggregate world)
  {
    if (world.OwnerId != user.Id)
    {
      throw new PermissionDeniedException(Action.Delete, EntityType.World, user, entityId: world.Id.ToGuid());
    }
  }

  public void EnsureCanUpdateWorld(User user, WorldAggregate world)
  {
    if (world.OwnerId != user.Id)
    {
      throw new PermissionDeniedException(Action.Update, EntityType.World, user, entityId: world.Id.ToGuid());
    }
  }
}
