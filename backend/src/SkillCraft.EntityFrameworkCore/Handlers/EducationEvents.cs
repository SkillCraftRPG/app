using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillCraft.Domain.Educations;
using SkillCraft.EntityFrameworkCore.Entities;

namespace SkillCraft.EntityFrameworkCore.Handlers;

internal static class EducationEvents
{
  public class CreatedEventHandler : INotificationHandler<Education.CreatedEvent>
  {
    private readonly SkillCraftContext _context;

    public CreatedEventHandler(SkillCraftContext context)
    {
      _context = context;
    }

    public async Task Handle(Education.CreatedEvent @event, CancellationToken cancellationToken)
    {
      EducationEntity? education = await _context.Educations.AsNoTracking()
        .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken);
      if (education == null)
      {
        Guid worldId = new EducationId(@event.AggregateId).WorldId.ToGuid();
        WorldEntity world = await _context.Worlds
          .SingleOrDefaultAsync(x => x.Id == worldId, cancellationToken)
          ?? throw new InvalidOperationException($"The world entity 'Id={worldId}' could not be found.");

        education = new(world, @event);

        _context.Educations.Add(education);

        await _context.SaveChangesAsync(cancellationToken);
      }
    }
  }

  public class UpdatedEventHandler : INotificationHandler<Education.UpdatedEvent>
  {
    private readonly SkillCraftContext _context;

    public UpdatedEventHandler(SkillCraftContext context)
    {
      _context = context;
    }

    public async Task Handle(Education.UpdatedEvent @event, CancellationToken cancellationToken)
    {
      EducationEntity education = await _context.Educations
        .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken)
        ?? throw new InvalidOperationException($"The education entity 'AggregateId={@event.AggregateId}' could not be found.");

      education.Update(@event);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }
}
