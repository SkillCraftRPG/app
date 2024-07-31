using Logitar.Portal.Client;
using SkillCraft.Application;
using SkillCraft.Application.Logging;
using SkillCraft.EntityFrameworkCore.PostgreSQL;
using SkillCraft.EntityFrameworkCore.SqlServer;
using SkillCraft.Infrastructure;
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

    DatabaseProvider databaseProvider = _configuration.GetValue<DatabaseProvider?>("DatabaseProvider") ?? DatabaseProvider.EntityFrameworkCoreSqlServer;
    switch (databaseProvider)
    {
      case DatabaseProvider.EntityFrameworkCorePostgreSQL:
        services.AddSkillCraftWithEntityFrameworkCorePostgreSQL(_configuration);
        break;
      case DatabaseProvider.EntityFrameworkCoreSqlServer:
        services.AddSkillCraftWithEntityFrameworkCoreSqlServer(_configuration);
        break;
      default:
        throw new DatabaseProviderNotSupportedException(databaseProvider);
    }

    services.AddSingleton<IActivityContextResolver, SeedingActivityContextResolver>();
    services.AddSingleton<ILogRepository, SeedingLogRepository>();
  }
}
