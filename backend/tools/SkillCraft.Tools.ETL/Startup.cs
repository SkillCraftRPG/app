using Logitar.Net.Http;

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
    services.AddHostedService<EtlWorker>();
    services.AddMediatR(config => config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
    services.AddSingleton<IApiClient, ApiClient>();

    string baseUrl = _configuration.GetValue<string>("BaseUrl") ?? throw new InvalidOperationException("The configuration 'BaseUrl' is required.");
    string token = _configuration.GetValue<string>("BearerToken") ?? throw new InvalidOperationException("The configuration 'BearerToken' is required.");
    HttpApiSettings apiSettings = new()
    {
      Authorization = HttpAuthorization.Bearer(token),
      BaseUri = new(baseUrl, UriKind.Absolute)
    };
    // TODO(fpion): Headers
    services.AddSingleton<IHttpApiSettings>(apiSettings);
  }
}
