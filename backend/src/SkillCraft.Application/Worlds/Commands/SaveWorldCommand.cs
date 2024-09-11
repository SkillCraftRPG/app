using Logitar.EventSourcing;
using MediatR;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Worlds.Commands;

internal record SaveWorldCommand(World World) : IRequest;

internal class SaveWorldCommandHandler : IRequestHandler<SaveWorldCommand>
{
  private readonly IWorldRepository _worldRepository;

  public SaveWorldCommandHandler(IWorldRepository worldRepository)
  {
    _worldRepository = worldRepository;
  }

  public async Task Handle(SaveWorldCommand command, CancellationToken cancellationToken)
  {
    World world = command.World;

    bool slugHasChanged = false;
    foreach (DomainEvent change in world.Changes)
    {
      if (change is World.CreatedEvent || change is World.UpdatedEvent updatedEvent && updatedEvent.Slug != null)
      {
        slugHasChanged = true;
        break;
      }
    }

    if (slugHasChanged)
    {
      World? other = await _worldRepository.LoadAsync(world.Slug, cancellationToken);
      if (other != null && !other.Equals(world))
      {
        throw new SlugAlreadyUsedException(world);
      }
    }

    // TODO(fpion): ensure has enough storage

    await _worldRepository.SaveAsync(world, cancellationToken);

    // TODO(fpion): update storage
  }
}
