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
      Guid ownerId = @event.AggregateId.ToGuid();
      StorageSummaryEntity summary = await _context.StorageSummaries
        .SingleOrDefaultAsync(x => x.OwnerId == ownerId, cancellationToken)
        ?? throw new InvalidOperationException($"The storage summary 'OwnerId={ownerId}' could not be found.");
      summary.Store(@event);

      StorageDetailEntity? detail = await _context.StorageDetails
        .SingleOrDefaultAsync(x => x.EntityType == @event.Key.Type && x.EntityId == @event.Key.Id, cancellationToken);
      if (detail == null)
      {
        Guid worldId = @event.Entity.WorldId.ToGuid();
        WorldEntity world = await _context.Worlds
          .SingleOrDefaultAsync(x => x.Id == worldId, cancellationToken)
          ?? throw new InvalidOperationException($"The world entity 'Id={worldId}' could not be found.");

        detail = new(world, @event);

        _context.StorageDetails.Add(detail);
      }
      else
      {
        detail.Update(@event);
      }

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
