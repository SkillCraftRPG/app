using Logitar.Net.Http;
using SkillCraft.Tools.ETL.Settings;

namespace SkillCraft.Tools.ETL;

internal class Startup
{
  private readonly IConfiguration _configuration;

  public Startup(IConfiguration configuration)
  {
    _configuration = configuration;
  }

  public void ConfigureServices(IServiceCollection services)
  {
    ApiSettings settings = _configuration.GetSection(ApiSettings.SectionKey).Get<ApiSettings>() ?? new();

    services.AddHostedService<EtlWorker>();
    services.AddMediatR(config => config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
    services.AddSingleton<IApiClient, ApiClient>();

    HttpApiSettings apiSettings = new()
    {
      Authorization = HttpAuthorization.Basic(settings.Basic.GetCredentials()),
      BaseUri = settings.BaseUri
    };
    services.AddSingleton<IHttpApiSettings>(apiSettings);
  }
}
