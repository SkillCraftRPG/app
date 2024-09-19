using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillCraft.Domain.Talents;
using SkillCraft.EntityFrameworkCore.Entities;

namespace SkillCraft.EntityFrameworkCore.Handlers;

internal static class TalentEvents
{
  public class CreatedEventHandler : INotificationHandler<Talent.CreatedEvent>
  {
    private readonly SkillCraftContext _context;

    public CreatedEventHandler(SkillCraftContext context)
    {
      _context = context;
    }

    public async Task Handle(Talent.CreatedEvent @event, CancellationToken cancellationToken)
    {
      TalentEntity? talent = await _context.Talents.AsNoTracking()
        .SingleOrDefaultAsync(x => x.Id == @event.AggregateId.ToGuid(), cancellationToken);
      if (talent == null)
      {
        Guid worldId = @event.WorldId.ToGuid();
        WorldEntity world = await _context.Worlds
          .SingleOrDefaultAsync(x => x.Id == worldId, cancellationToken)
          ?? throw new InvalidOperationException($"The world entity 'Id={worldId}' could not be found.");

        talent = new(world, @event);

        _context.Talents.Add(talent);

        await _context.SaveChangesAsync(cancellationToken);
      }
    }
  }

  public class UpdatedEventHandler : INotificationHandler<Talent.UpdatedEvent>
  {
    private readonly SkillCraftContext _context;

    public UpdatedEventHandler(SkillCraftContext context)
    {
      _context = context;
    }

    public async Task Handle(Talent.UpdatedEvent @event, CancellationToken cancellationToken)
    {
      Guid id = @event.AggregateId.ToGuid();
      TalentEntity talent = await _context.Talents
        .Include(x => x.RequiredTalent)
        .SingleOrDefaultAsync(x => x.Id == id, cancellationToken)
        ?? throw new InvalidOperationException($"The talent entity 'Id={id}' could not be found.");

      TalentEntity? requiredTalent = null;
      if (@event.RequiredTalentId?.Value != null)
      {
        Guid talentId = @event.RequiredTalentId.Value.Value.ToGuid();
        requiredTalent = await _context.Talents
          .SingleOrDefaultAsync(x => x.Id == talentId, cancellationToken)
          ?? throw new InvalidOperationException($"The talent entity 'Id={talentId}' could not be found.");
      }

      talent.Update(@event, requiredTalent);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }
}
