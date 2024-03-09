using GraphQL;
using GraphQL.Execution;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SkillCraft.GraphQL;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddSkillCraftGraphQL(this IServiceCollection services, IConfiguration configuration)
  {
    GraphQLSettings settings = configuration.GetSection("GraphQL").Get<GraphQLSettings>() ?? new();
    return services.AddSkillCraftGraphQL(settings);
  }
  public static IServiceCollection AddSkillCraftGraphQL(this IServiceCollection services, IGraphQLSettings settings)
  {
    return services.AddGraphQL(builder => builder
      .AddAuthorizationRule()
      .AddSchema<SkillCraftSchema>()
      .AddSystemTextJson()
      .AddErrorInfoProvider(new ErrorInfoProvider(options => options.ExposeExceptionDetails = settings.ExposeExceptionDetails))
      .AddGraphTypes(Assembly.GetExecutingAssembly())
      .ConfigureExecutionOptions(options => options.EnableMetrics = settings.EnableMetrics));
  }
}
