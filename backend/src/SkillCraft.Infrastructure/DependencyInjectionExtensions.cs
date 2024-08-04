using Logitar.EventSourcing.Infrastructure;
using Logitar.Identity.Infrastructure.Converters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SkillCraft.Application;
using SkillCraft.Application.Accounts;
using SkillCraft.Application.Caching;
using SkillCraft.Infrastructure.Caching;
using SkillCraft.Infrastructure.Converters;
using SkillCraft.Infrastructure.IdentityServices;
using SkillCraft.Infrastructure.Settings;

namespace SkillCraft.Infrastructure;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddSkillCraftInfrastructure(this IServiceCollection services)
  {
    return services
      .AddLogitarEventSourcingInfrastructure()
      .AddIdentityServices()
      .AddMemoryCache()
      .AddSkillCraftApplication()
      .AddSingleton(InitializeCachingSettings)
      .AddSingleton<ICacheService, CacheService>()
      .AddSingleton<IEventSerializer>(InitializeEventSerializer)
      .AddTransient<IEventBus, EventBus>();
  }

  private static IServiceCollection AddIdentityServices(this IServiceCollection services)
  {
    return services
      .AddTransient<IApiKeyService, ApiKeyService>()
      .AddTransient<IGoogleService, GoogleService>()
      .AddTransient<IMessageService, MessageService>()
      .AddTransient<IOneTimePasswordService, OneTimePasswordService>()
      .AddTransient<IRealmService, RealmService>()
      .AddTransient<ISessionService, SessionService>()
      .AddTransient<ITokenService, TokenService>()
      .AddTransient<IUserService, UserService>();
  }

  private static CachingSettings InitializeCachingSettings(IServiceProvider serviceProvider)
  {
    IConfiguration configuration = serviceProvider.GetRequiredService<IConfiguration>();
    return configuration.GetSection(CachingSettings.SectionKey).Get<CachingSettings>() ?? new();
  }

  private static EventSerializer InitializeEventSerializer(IServiceProvider serviceProvider) => new(serviceProvider.GetJsonConverters());
  public static IEnumerable<JsonConverter> GetJsonConverters(this IServiceProvider _) =>
  [
    new DescriptionConverter(),
    new DisplayNameConverter(),
    new SlugUnitConverter(),
    new WorldIdConverter()
  ];
}
