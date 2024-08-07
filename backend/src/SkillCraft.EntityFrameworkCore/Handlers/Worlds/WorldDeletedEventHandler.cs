using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillCraft.Domain.Worlds.Events;
using SkillCraft.EntityFrameworkCore.Entities;

namespace SkillCraft.EntityFrameworkCore.Handlers.Worlds;

internal class WorldDeletedEventHandler : INotificationHandler<WorldDeletedEvent>
{
  private readonly SkillCraftContext _context;

  public WorldDeletedEventHandler(SkillCraftContext context)
  {
    _context = context;
  }

  public async Task Handle(WorldDeletedEvent @event, CancellationToken cancellationToken)
  {
    WorldEntity? world = await _context.Worlds.SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken);
    if (world != null)
    {
      _context.Worlds.Remove(world);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }
}
