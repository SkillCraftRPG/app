﻿using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillCraft.Domain.Customizations;
using SkillCraft.EntityFrameworkCore.Entities;

namespace SkillCraft.EntityFrameworkCore.Handlers;

internal class CustomizationEvents : INotificationHandler<Customization.CreatedEvent>, INotificationHandler<Customization.UpdatedEvent>
{
  private readonly SkillCraftContext _context;

  public CustomizationEvents(SkillCraftContext context)
  {
    _context = context;
  }

  public async Task Handle(Customization.CreatedEvent @event, CancellationToken cancellationToken)
  {
    CustomizationEntity? customization = await _context.Customizations.AsNoTracking()
      .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken);
    if (customization == null)
    {
      Guid worldId = new CustomizationId(@event.AggregateId).WorldId.ToGuid();
      WorldEntity world = await _context.Worlds
        .SingleOrDefaultAsync(x => x.Id == worldId, cancellationToken)
        ?? throw new InvalidOperationException($"The world entity 'Id={worldId}' could not be found.");

      customization = new(world, @event);

      _context.Customizations.Add(customization);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }

  public async Task Handle(Customization.UpdatedEvent @event, CancellationToken cancellationToken)
  {
    CustomizationEntity customization = await _context.Customizations
      .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The customization entity 'AggregateId={@event.AggregateId}' could not be found.");

    customization.Update(@event);

    await _context.SaveChangesAsync(cancellationToken);
  }
}
