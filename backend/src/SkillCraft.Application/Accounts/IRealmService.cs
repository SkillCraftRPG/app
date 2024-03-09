using Logitar.Identity.Contracts.Settings;

namespace SkillCraft.Application.Accounts;

public interface IRealmService
{
  Task<IUserSettings> GetUserSettingsAsync(CancellationToken cancellationToken = default);
}
