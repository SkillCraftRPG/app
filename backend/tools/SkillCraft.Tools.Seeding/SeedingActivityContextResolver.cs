using SkillCraft.Application;

namespace SkillCraft.Tools.Seeding;

internal class SeedingActivityContextResolver : IActivityContextResolver
{
  public Task<Application.ActivityContext> ResolveAsync(CancellationToken cancellationToken)
  {
    Application.ActivityContext context = new(ApiKey: null, Session: null, User: null);
    return Task.FromResult(context);
  }
}
