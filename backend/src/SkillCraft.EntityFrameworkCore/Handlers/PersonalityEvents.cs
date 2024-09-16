using MediatR;
using Microsoft.EntityFrameworkCore;
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
        .SingleOrDefaultAsync(x => x.Id == @event.AggregateId.ToGuid(), cancellationToken);
      if (personality == null)
      {
        Guid worldId = @event.WorldId.ToGuid();
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
      Guid id = @event.AggregateId.ToGuid();
      PersonalityEntity personality = await _context.Personalities
        .SingleOrDefaultAsync(x => x.Id == id, cancellationToken)
        ?? throw new InvalidOperationException($"The personality entity 'Id={id}' could not be found.");

      CustomizationEntity? gift = null;
      if (@event.GiftId?.Value != null)
      {
        Guid giftId = @event.GiftId.Value.Value.ToGuid();
        gift = await _context.Customizations
          .SingleOrDefaultAsync(x => x.Id == giftId, cancellationToken)
          ?? throw new InvalidOperationException($"The customization entity 'Id={giftId}' could not be found.");
      }

      personality.Update(@event, gift);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }
}
