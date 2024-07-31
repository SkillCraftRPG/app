﻿using Logitar.Portal.Contracts.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SkillCraft.Application.Logging;

namespace SkillCraft.Application;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddSkillCraftApplication(this IServiceCollection services)
  {
    return services
      .AddMediatR(config => config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()))
      .AddSingleton<ILoggingSettings>(InitializeLoggingSettings)
      .AddScoped<ILoggingService, LoggingService>()
      .AddTransient<IRequestPipeline, RequestPipeline>();
  }

  private static LoggingSettings InitializeLoggingSettings(IServiceProvider serviceProvider)
  {
    IConfiguration configuration = serviceProvider.GetRequiredService<IConfiguration>();
    return configuration.GetSection("ApplicationLogging").Get<LoggingSettings>() ?? new();
  }
}