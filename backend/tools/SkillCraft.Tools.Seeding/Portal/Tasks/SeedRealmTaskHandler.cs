using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Realms;
using MediatR;

namespace SkillCraft.Tools.Seeding.Portal.Tasks;

internal class SeedRealmTaskHandler : INotificationHandler<SeedRealmTask>
{
  private readonly ILogger<SeedRealmTaskHandler> _logger;
  private readonly IRealmClient _realms;
  private readonly string _uniqueSlug;

  public SeedRealmTaskHandler(IConfiguration configuration, ILogger<SeedRealmTaskHandler> logger, IRealmClient realms)
  {
    _logger = logger;
    _realms = realms;
    _uniqueSlug = configuration.GetValue<string>("Portal:Realm") ?? throw new ArgumentException("The configuration 'Portal:Realm' is required.", nameof(configuration));
  }

  public async Task Handle(SeedRealmTask _, CancellationToken cancellationToken)
  {
    RequestContext context = new(cancellationToken);

    string status = "updated";
    Realm? realm = await _realms.ReadAsync(id: null, _uniqueSlug, context);
    if (realm == null)
    {
      CreateRealmPayload createPayload = new(_uniqueSlug, secret: string.Empty);
      realm = await _realms.CreateAsync(createPayload, context);
      status = "created";
    }

    ReplaceRealmPayload replacePayload = new(realm.UniqueSlug, realm.Secret)
    {
      DisplayName = "SkillCraftRPG",
      Description = "This is the realm of the SkillCraft Tabletop Role-Playing Game.",
      DefaultLocale = "fr",
      Url = "https://www.skillcraftrpg.ca"
    };
    realm = await _realms.ReplaceAsync(realm.Id, replacePayload, realm.Version, context)
      ?? throw new InvalidOperationException($"The realm 'Id={realm.Id}' replace result should not be null.");

    WorkerPortalSettings.Instance.SetRealm(realm);

    _logger.LogInformation("The realm '{UniqueSlug}' has been {Status} (Id={Id}).", realm.UniqueSlug, status, realm.Id);
  }
}
