using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillCraft.EntityFrameworkCore;

namespace SkillCraft.Tools.Seeding.Backend.Tasks;

internal class MigrateDatabaseTaskHandler : INotificationHandler<MigrateDatabaseTask>
{
  private readonly EventContext _eventContext;
  private readonly SkillCraftContext _skillCraftContext;

  public MigrateDatabaseTaskHandler(EventContext eventContext, SkillCraftContext skillCraftContext)
  {
    _eventContext = eventContext;
    _skillCraftContext = skillCraftContext;
  }

  public async Task Handle(MigrateDatabaseTask notification, CancellationToken cancellationToken)
  {
    await _eventContext.Database.MigrateAsync(cancellationToken);
    await _skillCraftContext.Database.MigrateAsync(cancellationToken);
  }
}
