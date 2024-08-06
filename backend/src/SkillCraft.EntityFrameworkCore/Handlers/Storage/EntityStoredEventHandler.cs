using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillCraft.Domain.Storage.Events;
using SkillCraft.EntityFrameworkCore.Entities;

namespace SkillCraft.EntityFrameworkCore.Handlers.Storage;

internal class EntityStoredEventHandler : INotificationHandler<EntityStoredEvent>
{
  private readonly SkillCraftContext _context;

  public EntityStoredEventHandler(SkillCraftContext context)
  {
    _context = context;
  }

  public async Task Handle(EntityStoredEvent @event, CancellationToken cancellationToken)
  {
    WorldEntity world = await _context.Worlds
      .SingleOrDefaultAsync(x => x.AggregateId == @event.WorldId.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The world entity 'AggregateId={@event.WorldId.AggregateId}' could not be found.");

    StorageDetailEntity? entity = await _context.StorageDetails
      .SingleOrDefaultAsync(x => x.EntityType == @event.EntityType && x.EntityId == @event.EntityId, cancellationToken);
    if (entity == null)
    {
      entity = new(world, @event);

      _context.StorageDetails.Add(entity);
    }
    else
    {
      entity.Update(@event);
    }

    StorageSummaryEntity summary = await _context.StorageSummaries
      .SingleOrDefaultAsync(x => x.UserId == world.OwnerId, cancellationToken)
      ?? throw new InvalidOperationException($"The storage summary entity 'UserId={world.OwnerId}' could not be found.");
    summary.Update(@event);

    await _context.SaveChangesAsync(cancellationToken);
  }
}
