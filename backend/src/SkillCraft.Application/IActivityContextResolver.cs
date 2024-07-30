namespace SkillCraft.Application;

public interface IActivityContextResolver
{
  Task<ActivityContext> ResolveAsync(CancellationToken cancellationToken = default);
}
