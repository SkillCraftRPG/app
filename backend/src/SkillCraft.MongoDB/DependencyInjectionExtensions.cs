using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using SkillCraft.Application.Logging;
using SkillCraft.Infrastructure;
using SkillCraft.MongoDB.Repositories;
using SkillCraft.MongoDB.Settings;

namespace SkillCraft.MongoDB;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddSkillCraftWithMongoDB(this IServiceCollection services, IConfiguration configuration)
  {
    MongoDBSettings settings = configuration.GetSection(MongoDBSettings.SectionKey).Get<MongoDBSettings>() ?? new();
    return services.AddSkillCraftWithMongoDB(settings);
  }
  public static IServiceCollection AddSkillCraftWithMongoDB(this IServiceCollection services, MongoDBSettings settings)
  {
    if (!string.IsNullOrWhiteSpace(settings.ConnectionString) && !string.IsNullOrWhiteSpace(settings.DatabaseName))
    {
      MongoClient client = new(settings.ConnectionString.Trim());
      IMongoDatabase database = client.GetDatabase(settings.DatabaseName.Trim());
      services.AddSingleton(database).AddTransient<ILogRepository, LogRepository>();
    }

    return services.AddSkillCraftInfrastructure();
  }
}
