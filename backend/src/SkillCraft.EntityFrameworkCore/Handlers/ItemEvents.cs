using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillCraft.Domain.Items;
using SkillCraft.EntityFrameworkCore.Entities;

namespace SkillCraft.EntityFrameworkCore.Handlers;

internal static class ItemEvents
{
  public class CreatedEventHandler : INotificationHandler<Item.CreatedEvent>
  {
    private readonly SkillCraftContext _context;

    public CreatedEventHandler(SkillCraftContext context)
    {
      _context = context;
    }

    public async Task Handle(Item.CreatedEvent @event, CancellationToken cancellationToken)
    {
      ItemEntity? item = await _context.Items.AsNoTracking()
        .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken);
      if (item == null)
      {
        Guid worldId = new ItemId(@event.AggregateId).WorldId.ToGuid();
        WorldEntity world = await _context.Worlds
          .SingleOrDefaultAsync(x => x.Id == worldId, cancellationToken)
          ?? throw new InvalidOperationException($"The world entity 'Id={worldId}' could not be found.");

        item = new(world, @event);

        _context.Items.Add(item);

        await _context.SaveChangesAsync(cancellationToken);
      }
    }
  }

  public class UpdatedEventHandler : INotificationHandler<Item.UpdatedEvent>
  {
    private readonly SkillCraftContext _context;

    public UpdatedEventHandler(SkillCraftContext context)
    {
      _context = context;
    }

    public async Task Handle(Item.UpdatedEvent @event, CancellationToken cancellationToken)
    {
      ItemEntity item = await _context.Items
        .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken)
        ?? throw new InvalidOperationException($"The item entity 'AggregateId={@event.AggregateId}' could not be found.");

      item.Update(@event);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }
}
