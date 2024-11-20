using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillCraft.Domain.Customizations;
using SkillCraft.Domain.Natures;
using SkillCraft.EntityFrameworkCore.Entities;

namespace SkillCraft.EntityFrameworkCore.Handlers;

internal static class NatureEvents
{
  public class CreatedEventHandler : INotificationHandler<Nature.CreatedEvent>
  {
    private readonly SkillCraftContext _context;

    public CreatedEventHandler(SkillCraftContext context)
    {
      _context = context;
    }

    public async Task Handle(Nature.CreatedEvent @event, CancellationToken cancellationToken)
    {
      NatureEntity? nature = await _context.Natures.AsNoTracking()
        .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken);
      if (nature == null)
      {
        Guid worldId = new NatureId(@event.AggregateId).WorldId.ToGuid();
        WorldEntity world = await _context.Worlds
          .SingleOrDefaultAsync(x => x.Id == worldId, cancellationToken)
          ?? throw new InvalidOperationException($"The world entity 'Id={worldId}' could not be found.");

        nature = new(world, @event);

        _context.Natures.Add(nature);

        await _context.SaveChangesAsync(cancellationToken);
      }
    }
  }

  public class UpdatedEventHandler : INotificationHandler<Nature.UpdatedEvent>
  {
    private readonly SkillCraftContext _context;

    public UpdatedEventHandler(SkillCraftContext context)
    {
      _context = context;
    }

    public async Task Handle(Nature.UpdatedEvent @event, CancellationToken cancellationToken)
    {
      NatureEntity nature = await _context.Natures
        .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken)
        ?? throw new InvalidOperationException($"The nature entity 'AggregateId={@event.AggregateId}' could not be found.");

      CustomizationEntity? gift = null;
      if (@event.GiftId?.Value != null)
      {
        CustomizationId giftId = @event.GiftId.Value.Value;
        gift = await _context.Customizations
          .SingleOrDefaultAsync(x => x.AggregateId == giftId.Value, cancellationToken)
          ?? throw new InvalidOperationException($"The customization entity 'AggregateId={giftId}' could not be found.");
      }

      nature.Update(@event, gift);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }
}
