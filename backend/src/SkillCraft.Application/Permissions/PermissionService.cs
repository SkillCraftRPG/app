using Logitar.Portal.Contracts.Users;
using SkillCraft.Application.Settings;
using SkillCraft.Application.Worlds;

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
    await Task.Delay(1, cancellationToken); // TODO(fpion): implement
  }
}
