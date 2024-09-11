using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillCraft.Domain.Worlds;
using SkillCraft.EntityFrameworkCore.Entities;

namespace SkillCraft.EntityFrameworkCore.Handlers;

internal static class WorldEvents
{
  public class CreatedEventHandler : INotificationHandler<World.CreatedEvent>
  {
    private readonly SkillCraftContext _context;

    public CreatedEventHandler(SkillCraftContext context)
    {
      _context = context;
    }

    public async Task Handle(World.CreatedEvent @event, CancellationToken cancellationToken)
    {
      WorldEntity? world = await _context.Worlds.AsNoTracking()
        .SingleOrDefaultAsync(x => x.Id == @event.AggregateId.ToGuid(), cancellationToken);
      if (world == null)
      {
        Guid userId = @event.OwnerId.ToGuid();
        UserEntity owner = await _context.Users
          .SingleOrDefaultAsync(x => x.Id == userId, cancellationToken)
          ?? throw new InvalidOperationException($"The user entity 'Id={userId}' could not be found.");

        world = new(owner, @event);

        _context.Worlds.Add(world);

        await _context.SaveChangesAsync(cancellationToken);
      }
    }
  }

  public class UpdatedEventHandler : INotificationHandler<World.UpdatedEvent>
  {
    private readonly SkillCraftContext _context;

    public UpdatedEventHandler(SkillCraftContext context)
    {
      _context = context;
    }

    public async Task Handle(World.UpdatedEvent @event, CancellationToken cancellationToken)
    {
      Guid id = @event.AggregateId.ToGuid();
      WorldEntity world = await _context.Worlds
        .SingleOrDefaultAsync(x => x.Id == id, cancellationToken)
        ?? throw new InvalidOperationException($"The world entity 'Id={id}' could not be found.");

      world.Update(@event);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }
}
