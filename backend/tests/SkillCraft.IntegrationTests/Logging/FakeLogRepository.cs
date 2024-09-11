using SkillCraft.Application.Logging;

namespace SkillCraft.Logging;

internal class FakeLogRepository : ILogRepository
{
  public Task SaveAsync(Log log, CancellationToken cancellationToken) => Task.CompletedTask;
}
