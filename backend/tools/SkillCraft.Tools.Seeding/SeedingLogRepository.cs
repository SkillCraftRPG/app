using SkillCraft.Application.Logging;

namespace SkillCraft.Tools.Seeding;

internal class SeedingLogRepository : ILogRepository
{
  public Task SaveAsync(Log log, CancellationToken cancellationToken)
  {
    return Task.CompletedTask;
  }
}
