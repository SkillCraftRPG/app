using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillCraft.Domain.Parties;
using SkillCraft.EntityFrameworkCore.Entities;

namespace SkillCraft.EntityFrameworkCore.Handlers;

internal static class PartyEvents
{
  public class CreatedEventHandler : INotificationHandler<Party.CreatedEvent>
  {
    private readonly SkillCraftContext _context;

    public CreatedEventHandler(SkillCraftContext context)
    {
      _context = context;
    }

    public async Task Handle(Party.CreatedEvent @event, CancellationToken cancellationToken)
    {
      PartyEntity? party = await _context.Parties.AsNoTracking()
        .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken);
      if (party == null)
      {
        Guid worldId = new PartyId(@event.AggregateId).WorldId.ToGuid();
        WorldEntity world = await _context.Worlds
          .SingleOrDefaultAsync(x => x.Id == worldId, cancellationToken)
          ?? throw new InvalidOperationException($"The world entity 'Id={worldId}' could not be found.");

        party = new(world, @event);

        _context.Parties.Add(party);

        await _context.SaveChangesAsync(cancellationToken);
      }
    }
  }

  public class UpdatedEventHandler : INotificationHandler<Party.UpdatedEvent>
  {
    private readonly SkillCraftContext _context;

    public UpdatedEventHandler(SkillCraftContext context)
    {
      _context = context;
    }

    public async Task Handle(Party.UpdatedEvent @event, CancellationToken cancellationToken)
    {
      PartyEntity party = await _context.Parties
        .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken)
        ?? throw new InvalidOperationException($"The party entity 'AggregateId={@event.AggregateId}' could not be found.");

      party.Update(@event);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }
}
