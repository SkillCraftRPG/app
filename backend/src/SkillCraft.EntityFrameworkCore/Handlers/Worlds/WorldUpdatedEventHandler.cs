using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillCraft.Domain.Worlds.Events;
using SkillCraft.EntityFrameworkCore.Entities;

namespace SkillCraft.EntityFrameworkCore.Handlers.Worlds;

internal class WorldUpdatedEventHandler : INotificationHandler<WorldUpdatedEvent>
{
  private readonly SkillCraftContext _context;

  public WorldUpdatedEventHandler(SkillCraftContext context)
  {
    _context = context;
  }

  public async Task Handle(WorldUpdatedEvent @event, CancellationToken cancellationToken)
  {
    WorldEntity world = await _context.Worlds
      .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The world entity 'AggregateId={@event.AggregateId}' could not be found.");

    world.Update(@event);

    await _context.SaveChangesAsync(cancellationToken);
  }
}
