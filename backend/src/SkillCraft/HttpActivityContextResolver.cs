using SkillCraft.Application;
using SkillCraft.Extensions;

namespace SkillCraft;

internal class HttpActivityContextResolver : IActivityContextResolver
{
  private readonly IHttpContextAccessor _httpContextAccessor;
  protected HttpContext Context => _httpContextAccessor.HttpContext ?? throw new InvalidOperationException($"The {nameof(_httpContextAccessor.HttpContext)} is required.");

  public HttpActivityContextResolver(IHttpContextAccessor httpContextAccessor)
  {
    _httpContextAccessor = httpContextAccessor;
  }

  public Task<ActivityContext> ResolveAsync(CancellationToken cancellationToken)
  {
    ActivityContext context = new(Context.GetApiKey(), Context.GetSession(), Context.GetUser(), World: null);
    return Task.FromResult(context);
  }
}
