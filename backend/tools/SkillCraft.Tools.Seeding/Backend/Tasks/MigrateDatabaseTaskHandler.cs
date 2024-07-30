using MediatR;

namespace SkillCraft.Tools.Seeding.Backend.Tasks;

internal class MigrateDatabaseTaskHandler : INotificationHandler<MigrateDatabaseTask>
{
  public Task Handle(MigrateDatabaseTask notification, CancellationToken cancellationToken)
  {
    return Task.CompletedTask; // TODO(fpion): implement
  }
}
