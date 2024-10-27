using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillCraft.Domain.Items;
using SkillCraft.EntityFrameworkCore.Entities;

namespace SkillCraft.EntityFrameworkCore.Handlers;

internal static class ItemEvents
{
  public class ConsumablePropertiesUpdatedEventHandler : INotificationHandler<Item.ConsumablePropertiesUpdatedEvent>
  {
    private readonly SkillCraftContext _context;

    public ConsumablePropertiesUpdatedEventHandler(SkillCraftContext context)
    {
      _context = context;
    }

    public async Task Handle(Item.ConsumablePropertiesUpdatedEvent @event, CancellationToken cancellationToken)
    {
      ItemEntity item = await _context.Items
        .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken)
        ?? throw new InvalidOperationException($"The item entity 'AggregateId={@event.AggregateId}' could not be found.");

      item.SetProperties(@event);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }

  public class ContainerPropertiesUpdatedEventHandler : INotificationHandler<Item.ContainerPropertiesUpdatedEvent>
  {
    private readonly SkillCraftContext _context;

    public ContainerPropertiesUpdatedEventHandler(SkillCraftContext context)
    {
      _context = context;
    }

    public async Task Handle(Item.ContainerPropertiesUpdatedEvent @event, CancellationToken cancellationToken)
    {
      ItemEntity item = await _context.Items
        .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken)
        ?? throw new InvalidOperationException($"The item entity 'AggregateId={@event.AggregateId}' could not be found.");

      item.SetProperties(@event);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }

  public class CreatedEventHandler : INotificationHandler<Item.CreatedEvent>
  {
    private readonly SkillCraftContext _context;

    public CreatedEventHandler(SkillCraftContext context)
    {
      _context = context;
    }

    public async Task Handle(Item.CreatedEvent @event, CancellationToken cancellationToken)
    {
      ItemEntity? item = await _context.Items.AsNoTracking()
        .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken);
      if (item == null)
      {
        Guid worldId = new ItemId(@event.AggregateId).WorldId.ToGuid();
        WorldEntity world = await _context.Worlds
          .SingleOrDefaultAsync(x => x.Id == worldId, cancellationToken)
          ?? throw new InvalidOperationException($"The world entity 'Id={worldId}' could not be found.");

        item = new(world, @event);

        _context.Items.Add(item);

        await _context.SaveChangesAsync(cancellationToken);
      }
    }
  }

  public class DevicePropertiesUpdatedEventHandler : INotificationHandler<Item.DevicePropertiesUpdatedEvent>
  {
    private readonly SkillCraftContext _context;

    public DevicePropertiesUpdatedEventHandler(SkillCraftContext context)
    {
      _context = context;
    }

    public async Task Handle(Item.DevicePropertiesUpdatedEvent @event, CancellationToken cancellationToken)
    {
      ItemEntity item = await _context.Items
        .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken)
        ?? throw new InvalidOperationException($"The item entity 'AggregateId={@event.AggregateId}' could not be found.");

      item.SetProperties(@event);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }

  public class EquipmentPropertiesUpdatedEventHandler : INotificationHandler<Item.EquipmentPropertiesUpdatedEvent>
  {
    private readonly SkillCraftContext _context;

    public EquipmentPropertiesUpdatedEventHandler(SkillCraftContext context)
    {
      _context = context;
    }

    public async Task Handle(Item.EquipmentPropertiesUpdatedEvent @event, CancellationToken cancellationToken)
    {
      ItemEntity item = await _context.Items
        .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken)
        ?? throw new InvalidOperationException($"The item entity 'AggregateId={@event.AggregateId}' could not be found.");

      item.SetProperties(@event);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }

  public class MiscellaneousPropertiesUpdatedEventHandler : INotificationHandler<Item.MiscellaneousPropertiesUpdatedEvent>
  {
    private readonly SkillCraftContext _context;

    public MiscellaneousPropertiesUpdatedEventHandler(SkillCraftContext context)
    {
      _context = context;
    }

    public async Task Handle(Item.MiscellaneousPropertiesUpdatedEvent @event, CancellationToken cancellationToken)
    {
      ItemEntity item = await _context.Items
        .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken)
        ?? throw new InvalidOperationException($"The item entity 'AggregateId={@event.AggregateId}' could not be found.");

      item.SetProperties(@event);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }

  public class MoneyPropertiesUpdatedEventHandler : INotificationHandler<Item.MoneyPropertiesUpdatedEvent>
  {
    private readonly SkillCraftContext _context;

    public MoneyPropertiesUpdatedEventHandler(SkillCraftContext context)
    {
      _context = context;
    }

    public async Task Handle(Item.MoneyPropertiesUpdatedEvent @event, CancellationToken cancellationToken)
    {
      ItemEntity item = await _context.Items
        .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken)
        ?? throw new InvalidOperationException($"The item entity 'AggregateId={@event.AggregateId}' could not be found.");

      item.SetProperties(@event);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }

  public class UpdatedEventHandler : INotificationHandler<Item.UpdatedEvent>
  {
    private readonly SkillCraftContext _context;

    public UpdatedEventHandler(SkillCraftContext context)
    {
      _context = context;
    }

    public async Task Handle(Item.UpdatedEvent @event, CancellationToken cancellationToken)
    {
      ItemEntity item = await _context.Items
        .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken)
        ?? throw new InvalidOperationException($"The item entity 'AggregateId={@event.AggregateId}' could not be found.");

      item.Update(@event);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }

  public class WeaponPropertiesUpdatedEventHandler : INotificationHandler<Item.WeaponPropertiesUpdatedEvent>
  {
    private readonly SkillCraftContext _context;

    public WeaponPropertiesUpdatedEventHandler(SkillCraftContext context)
    {
      _context = context;
    }

    public async Task Handle(Item.WeaponPropertiesUpdatedEvent @event, CancellationToken cancellationToken)
    {
      ItemEntity item = await _context.Items
        .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken)
        ?? throw new InvalidOperationException($"The item entity 'AggregateId={@event.AggregateId}' could not be found.");

      item.SetProperties(@event);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }
}
