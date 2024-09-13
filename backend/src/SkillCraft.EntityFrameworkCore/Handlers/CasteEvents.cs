using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillCraft.Domain.Castes;
using SkillCraft.EntityFrameworkCore.Entities;

namespace SkillCraft.EntityFrameworkCore.Handlers;

internal static class CasteEvents
{
  public class CreatedEventHandler : INotificationHandler<Caste.CreatedEvent>
  {
    private readonly SkillCraftContext _context;

    public CreatedEventHandler(SkillCraftContext context)
    {
      _context = context;
    }

    public async Task Handle(Caste.CreatedEvent @event, CancellationToken cancellationToken)
    {
      CasteEntity? caste = await _context.Castes.AsNoTracking()
        .SingleOrDefaultAsync(x => x.Id == @event.AggregateId.ToGuid(), cancellationToken);
      if (caste == null)
      {
        Guid worldId = @event.WorldId.ToGuid();
        WorldEntity world = await _context.Worlds
          .SingleOrDefaultAsync(x => x.Id == worldId, cancellationToken)
          ?? throw new InvalidOperationException($"The world entity 'Id={worldId}' could not be found.");

        caste = new(world, @event);

        _context.Castes.Add(caste);

        await _context.SaveChangesAsync(cancellationToken);
      }
    }
  }

  public class UpdatedEventHandler : INotificationHandler<Caste.UpdatedEvent>
  {
    private readonly SkillCraftContext _context;

    public UpdatedEventHandler(SkillCraftContext context)
    {
      _context = context;
    }

    public async Task Handle(Caste.UpdatedEvent @event, CancellationToken cancellationToken)
    {
      Guid id = @event.AggregateId.ToGuid();
      CasteEntity caste = await _context.Castes
        .SingleOrDefaultAsync(x => x.Id == id, cancellationToken)
        ?? throw new InvalidOperationException($"The caste entity 'Id={id}' could not be found.");

      caste.Update(@event);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }
}
