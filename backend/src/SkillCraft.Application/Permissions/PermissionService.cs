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
    int count = await _worldQuerier.CountOwnedAsync(user, cancellationToken);
    if (count >= _accountSettings.WorldLimit)
    {
      throw new NotImplementedException(); // TODO(fpion): implement
    }
  }
}
