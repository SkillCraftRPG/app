using Microsoft.AspNetCore.Authorization;
using SkillCraft.Extensions;

namespace SkillCraft.Authorization;

internal class UserAuthorizationHandler : AuthorizationHandler<UserAuthorizationRequirement>
{
  private readonly IHttpContextAccessor _httpContextAccessor;

  public UserAuthorizationHandler(IHttpContextAccessor httpContextAccessor)
  {
    _httpContextAccessor = httpContextAccessor;
  }

  protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, UserAuthorizationRequirement requirement)
  {
    HttpContext? httpContext = _httpContextAccessor.HttpContext;
    if (httpContext != null)
    {
      if (httpContext.GetUser() != null)
      {
        context.Succeed(requirement);
      }
    }

    return Task.CompletedTask;
  }
}
