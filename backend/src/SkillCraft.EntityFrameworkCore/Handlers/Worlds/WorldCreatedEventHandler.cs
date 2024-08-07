using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillCraft.Domain.Worlds.Events;
using SkillCraft.EntityFrameworkCore.Entities;

namespace SkillCraft.EntityFrameworkCore.Handlers.Worlds;

internal class WorldCreatedEventHandler : INotificationHandler<WorldCreatedEvent>
{
  private readonly SkillCraftContext _context;

  public WorldCreatedEventHandler(SkillCraftContext context)
  {
    _context = context;
  }

  public async Task Handle(WorldCreatedEvent @event, CancellationToken cancellationToken)
  {
    WorldEntity? world = await _context.Worlds.AsNoTracking()
      .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken);
    if (world == null)
    {
      world = new(@event);

      _context.Worlds.Add(world);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }
}
