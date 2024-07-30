using Microsoft.Extensions.DependencyInjection;
using SkillCraft.Application;
using SkillCraft.Application.Accounts;
using SkillCraft.Infrastructure.IdentityServices;

namespace SkillCraft.Infrastructure;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddSkillCraftInfrastructure(this IServiceCollection services)
  {
    return services
      .AddIdentityServices()
      .AddSkillCraftApplication();
  }

  private static IServiceCollection AddIdentityServices(this IServiceCollection services)
  {
    return services
      .AddTransient<IApiKeyService, ApiKeyService>()
      .AddTransient<IMessageService, MessageService>()
      .AddTransient<IOneTimePasswordService, OneTimePasswordService>()
      .AddTransient<ISessionService, SessionService>()
      .AddTransient<ITokenService, TokenService>()
      .AddTransient<IUserService, UserService>();
  }
}
