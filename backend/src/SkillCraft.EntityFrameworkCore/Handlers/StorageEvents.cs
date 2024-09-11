using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillCraft.Domain.Storages;
using SkillCraft.EntityFrameworkCore.Entities;

namespace SkillCraft.EntityFrameworkCore.Handlers;

internal static class StorageEvents
{
  public class EntityStoredEventHandler : INotificationHandler<Storage.EntityStoredEvent>
  {
    private readonly SkillCraftContext _context;

    public EntityStoredEventHandler(SkillCraftContext context)
    {
      _context = context;
    }

    public async Task Handle(Storage.EntityStoredEvent @event, CancellationToken cancellationToken)
    {
      StorageSummaryEntity summary = await _context.StorageSummaries
        .SingleOrDefaultAsync(x => x.OwnerId == @event.AggregateId.ToGuid(), cancellationToken)
        ?? throw new InvalidOperationException($"The storage summary '' could not be found.");

      summary.Store(@event);

      // TODO(fpion): StorageDetailEntity

      await _context.SaveChangesAsync(cancellationToken);
    }
  }

  public class InitializedEventHandler : INotificationHandler<Storage.InitializedEvent>
  {
    private readonly SkillCraftContext _context;

    public InitializedEventHandler(SkillCraftContext context)
    {
      _context = context;
    }

    public async Task Handle(Storage.InitializedEvent @event, CancellationToken cancellationToken)
    {
      Guid userId = @event.UserId.ToGuid();
      StorageSummaryEntity? summary = await _context.StorageSummaries.AsNoTracking()
        .SingleOrDefaultAsync(x => x.OwnerId == userId, cancellationToken);
      if (summary == null)
      {
        UserEntity owner = await _context.Users
          .SingleOrDefaultAsync(x => x.Id == userId, cancellationToken)
          ?? throw new InvalidOperationException($"The user entity 'Id={userId}' could not be found.");

        summary = new(owner, @event);

        _context.StorageSummaries.Add(summary);

        await _context.SaveChangesAsync(cancellationToken);
      }
    }
  }
}
