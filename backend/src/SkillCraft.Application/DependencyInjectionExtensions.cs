using Logitar.Portal.Contracts.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SkillCraft.Application.Logging;
using SkillCraft.Application.Permissions;
using SkillCraft.Application.Settings;
using SkillCraft.Application.Storage;

namespace SkillCraft.Application;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddSkillCraftApplication(this IServiceCollection services)
  {
    return services
      .AddMediatR(config => config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()))
      .AddSingleton<ILoggingSettings>(InitializeLoggingSettings)
      .AddSingleton(InitializeAccountSettings)
      .AddScoped<ILoggingService, LoggingService>()
      .AddTransient<IPermissionService, PermissionService>()
      .AddTransient<IRequestPipeline, RequestPipeline>()
      .AddTransient<IStorageService, StorageService>();
  }

  private static LoggingSettings InitializeLoggingSettings(IServiceProvider serviceProvider)
  {
    IConfiguration configuration = serviceProvider.GetRequiredService<IConfiguration>();
    return configuration.GetSection("ApplicationLogging").Get<LoggingSettings>() ?? new();
  }

  private static AccountSettings InitializeAccountSettings(IServiceProvider serviceProvider)
  {
    IConfiguration configuration = serviceProvider.GetRequiredService<IConfiguration>();
    return configuration.GetSection(AccountSettings.SectionKey).Get<AccountSettings>() ?? new();
  }
}
