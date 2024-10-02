using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillCraft.Domain.Customizations;
using SkillCraft.Domain.Personalities;
using SkillCraft.EntityFrameworkCore.Entities;

namespace SkillCraft.EntityFrameworkCore.Handlers;

internal static class PersonalityEvents
{
  public class CreatedEventHandler : INotificationHandler<Personality.CreatedEvent>
  {
    private readonly SkillCraftContext _context;

    public CreatedEventHandler(SkillCraftContext context)
    {
      _context = context;
    }

    public async Task Handle(Personality.CreatedEvent @event, CancellationToken cancellationToken)
    {
      PersonalityEntity? personality = await _context.Personalities.AsNoTracking()
        .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken);
      if (personality == null)
      {
        Guid worldId = new PersonalityId(@event.AggregateId).WorldId.ToGuid();
        WorldEntity world = await _context.Worlds
          .SingleOrDefaultAsync(x => x.Id == worldId, cancellationToken)
          ?? throw new InvalidOperationException($"The world entity 'Id={worldId}' could not be found.");

        personality = new(world, @event);

        _context.Personalities.Add(personality);

        await _context.SaveChangesAsync(cancellationToken);
      }
    }
  }

  public class UpdatedEventHandler : INotificationHandler<Personality.UpdatedEvent>
  {
    private readonly SkillCraftContext _context;

    public UpdatedEventHandler(SkillCraftContext context)
    {
      _context = context;
    }

    public async Task Handle(Personality.UpdatedEvent @event, CancellationToken cancellationToken)
    {
      PersonalityEntity personality = await _context.Personalities
        .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken)
        ?? throw new InvalidOperationException($"The personality entity 'AggregateId={@event.AggregateId}' could not be found.");

      CustomizationEntity? gift = null;
      if (@event.GiftId?.Value != null)
      {
        CustomizationId giftId = @event.GiftId.Value.Value;
        gift = await _context.Customizations
          .SingleOrDefaultAsync(x => x.AggregateId == giftId.Value, cancellationToken)
          ?? throw new InvalidOperationException($"The customization entity 'AggregateId={giftId}' could not be found.");
      }

      personality.Update(@event, gift);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }
}
