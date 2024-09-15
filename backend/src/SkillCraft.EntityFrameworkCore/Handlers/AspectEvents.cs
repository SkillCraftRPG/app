using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillCraft.Domain.Aspects;
using SkillCraft.EntityFrameworkCore.Entities;

namespace SkillCraft.EntityFrameworkCore.Handlers;

internal static class AspectEvents
{
  public class CreatedEventHandler : INotificationHandler<Aspect.CreatedEvent>
  {
    private readonly SkillCraftContext _context;

    public CreatedEventHandler(SkillCraftContext context)
    {
      _context = context;
    }

    public async Task Handle(Aspect.CreatedEvent @event, CancellationToken cancellationToken)
    {
      AspectEntity? aspect = await _context.Aspects.AsNoTracking()
        .SingleOrDefaultAsync(x => x.Id == @event.AggregateId.ToGuid(), cancellationToken);
      if (aspect == null)
      {
        Guid worldId = @event.WorldId.ToGuid();
        WorldEntity world = await _context.Worlds
          .SingleOrDefaultAsync(x => x.Id == worldId, cancellationToken)
          ?? throw new InvalidOperationException($"The world entity 'Id={worldId}' could not be found.");

        aspect = new(world, @event);

        _context.Aspects.Add(aspect);

        await _context.SaveChangesAsync(cancellationToken);
      }
    }
  }

  public class UpdatedEventHandler : INotificationHandler<Aspect.UpdatedEvent>
  {
    private readonly SkillCraftContext _context;

    public UpdatedEventHandler(SkillCraftContext context)
    {
      _context = context;
    }

    public async Task Handle(Aspect.UpdatedEvent @event, CancellationToken cancellationToken)
    {
      Guid id = @event.AggregateId.ToGuid();
      AspectEntity aspect = await _context.Aspects
        .SingleOrDefaultAsync(x => x.Id == id, cancellationToken)
        ?? throw new InvalidOperationException($"The aspect entity 'Id={id}' could not be found.");

      aspect.Update(@event);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }
}
