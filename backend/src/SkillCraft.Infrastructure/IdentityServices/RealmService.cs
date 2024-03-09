using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Domain.Settings;
using Logitar.Portal.Client;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Realms;
using SkillCraft.Application.Accounts;

namespace SkillCraft.Infrastructure.IdentityServices;

internal class RealmService : IRealmService
{
  private readonly IRealmClient _realmClient;
  private readonly IPortalSettings _settings;

  public RealmService(IRealmClient realmClient, IPortalSettings settings)
  {
    _realmClient = realmClient;
    _settings = settings;
  }

  public async Task<IUserSettings> GetUserSettingsAsync(CancellationToken cancellationToken)
  {
    bool isId = Guid.TryParse(_settings.Realm, out Guid id);

    RequestContext context = new(cancellationToken);
    Realm realm = await _realmClient.ReadAsync(isId ? id : null, uniqueSlug: _settings.Realm, context)
      ?? throw new InvalidOperationException($"The realm '{_settings.Realm}' could not be found.");

    UserSettings userSettings = new()
    {
      UniqueName = realm.UniqueNameSettings,
      Password = realm.PasswordSettings,
      RequireUniqueEmail = realm.RequireUniqueEmail
    };
    return userSettings;
  }
}
