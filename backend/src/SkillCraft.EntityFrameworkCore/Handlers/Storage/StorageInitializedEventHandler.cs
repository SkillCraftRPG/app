using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillCraft.Domain.Storage.Events;
using SkillCraft.EntityFrameworkCore.Entities;

namespace SkillCraft.EntityFrameworkCore.Handlers.Storage;

internal class StorageInitializedEventHandler : INotificationHandler<StorageInitializedEvent>
{
  private readonly SkillCraftContext _context;

  public StorageInitializedEventHandler(SkillCraftContext context)
  {
    _context = context;
  }

  public async Task Handle(StorageInitializedEvent @event, CancellationToken cancellationToken)
  {
    StorageSummaryEntity? entity = await _context.StorageSummaries.AsNoTracking()
      .SingleOrDefaultAsync(x => x.UserId == @event.UserId, cancellationToken);
    if (entity == null)
    {
      entity = new(@event);

      _context.StorageSummaries.Add(entity);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }
}
