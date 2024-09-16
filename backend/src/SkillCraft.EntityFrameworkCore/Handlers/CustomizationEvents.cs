using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillCraft.Domain.Customizations;
using SkillCraft.EntityFrameworkCore.Entities;

namespace SkillCraft.EntityFrameworkCore.Handlers;

internal static class CustomizationEvents
{
  public class CreatedEventHandler : INotificationHandler<Customization.CreatedEvent>
  {
    private readonly SkillCraftContext _context;

    public CreatedEventHandler(SkillCraftContext context)
    {
      _context = context;
    }

    public async Task Handle(Customization.CreatedEvent @event, CancellationToken cancellationToken)
    {
      CustomizationEntity? customization = await _context.Customizations.AsNoTracking()
        .SingleOrDefaultAsync(x => x.Id == @event.AggregateId.ToGuid(), cancellationToken);
      if (customization == null)
      {
        Guid worldId = @event.WorldId.ToGuid();
        WorldEntity world = await _context.Worlds
          .SingleOrDefaultAsync(x => x.Id == worldId, cancellationToken)
          ?? throw new InvalidOperationException($"The world entity 'Id={worldId}' could not be found.");

        customization = new(world, @event);

        _context.Customizations.Add(customization);

        await _context.SaveChangesAsync(cancellationToken);
      }
    }
  }

  public class UpdatedEventHandler : INotificationHandler<Customization.UpdatedEvent>
  {
    private readonly SkillCraftContext _context;

    public UpdatedEventHandler(SkillCraftContext context)
    {
      _context = context;
    }

    public async Task Handle(Customization.UpdatedEvent @event, CancellationToken cancellationToken)
    {
      Guid id = @event.AggregateId.ToGuid();
      CustomizationEntity customization = await _context.Customizations
        .SingleOrDefaultAsync(x => x.Id == id, cancellationToken)
        ?? throw new InvalidOperationException($"The customization entity 'Id={id}' could not be found.");

      customization.Update(@event);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }
}
