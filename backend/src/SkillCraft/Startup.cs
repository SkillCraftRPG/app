using SkillCraft.Extensions;
using SkillCraft.Filters;
using SkillCraft.Infrastructure;
using SkillCraft.Middlewares;
using SkillCraft.Settings;

namespace SkillCraft;

internal class Startup : StartupBase
{
  private readonly IConfiguration _configuration;
  private readonly bool _enableOpenApi;

  public Startup(IConfiguration configuration)
  {
    _configuration = configuration;
    _enableOpenApi = configuration.GetValue<bool>("EnableOpenApi");
  }

  public override void ConfigureServices(IServiceCollection services)
  {
    base.ConfigureServices(services);

    services.AddControllers(options => options.Filters.Add<ExceptionHandling>())
      .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

    // TODO(fpion): GraphQL

    CorsSettings corsSettings = _configuration.GetSection("Cors").Get<CorsSettings>() ?? new();
    services.AddSingleton(corsSettings);
    services.AddCors(corsSettings);

    // TODO(fpion): Authentication

    // TODO(fpion): Authorization

    CookiesSettings cookiesSettings = _configuration.GetSection("Cookies").Get<CookiesSettings>() ?? new();
    services.AddSingleton(cookiesSettings);
    services.AddSession(options =>
    {
      options.Cookie.SameSite = cookiesSettings.Session.SameSite;
      options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    });

    services.AddApplicationInsightsTelemetry();
    IHealthChecksBuilder healthChecks = services.AddHealthChecks();

    if (_enableOpenApi)
    {
      services.AddOpenApi();
    }

    // TODO(fpion): persistence

    services.AddDistributedMemoryCache();
    services.AddSkillCraftInfrastructure();
  }

  public override void Configure(IApplicationBuilder builder)
  {
    if (_enableOpenApi)
    {
      builder.UseOpenApi();
    }

    //if (_configuration.GetValue<bool>("UseGraphQLAltair"))
    //{
    //  builder.UseGraphQLAltair();
    //}
    //if (_configuration.GetValue<bool>("UseGraphQLGraphiQL"))
    //{
    //  builder.UseGraphQLGraphiQL();
    //}
    //if (_configuration.GetValue<bool>("UseGraphQLPlayground"))
    //{
    //  builder.UseGraphQLPlayground();
    //}
    //if (_configuration.GetValue<bool>("UseGraphQLVoyager"))
    //{
    //  builder.UseGraphQLVoyager();
    //} // TODO(fpion): GraphQL

    builder.UseHttpsRedirection();
    builder.UseCors();
    builder.UseStaticFiles();
    builder.UseSession();
    builder.UseMiddleware<RenewSession>();
    //builder.UseAuthentication(); // TODO(fpion): Authentication
    //builder.UseAuthorization(); // TODO(fpion): Authorization

    //builder.UseGraphQL<SkillCraftSchema>("/graphql", options => options.AuthenticationSchemes.AddRange(_authenticationSchemes)); // TODO(fpion): GraphQL

    if (builder is WebApplication application)
    {
      application.MapControllers();
      application.MapHealthChecks("/health");
    }
  }
}
