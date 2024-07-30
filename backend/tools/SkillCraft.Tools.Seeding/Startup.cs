using Logitar.Portal.Client;
using SkillCraft.Tools.Seeding.Portal;

namespace SkillCraft.Tools.Seeding;

internal class Startup
{
  private readonly IConfiguration _configuration;

  public Startup(IConfiguration configuration)
  {
    _configuration = configuration;
  }

  public void ConfigureServices(IServiceCollection services)
  {
    IPortalSettings portalSettings = _configuration.GetSection("Portal").Get<PortalSettings>() ?? new();
    portalSettings = WorkerPortalSettings.Initialize(portalSettings);
    services.AddLogitarPortalClient(portalSettings);

    services.AddMediatR(config => config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
    services.AddHostedService<SeedingWorker>();
  }
}
