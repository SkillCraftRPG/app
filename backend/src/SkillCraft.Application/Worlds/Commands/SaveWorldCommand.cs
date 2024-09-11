using Logitar.EventSourcing;
using MediatR;
using SkillCraft.Application.Storages;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Worlds.Commands;

internal record SaveWorldCommand(World World) : IRequest;

internal class SaveWorldCommandHandler : IRequestHandler<SaveWorldCommand>
{
  private readonly IStorageService _storageService;
  private readonly IWorldRepository _worldRepository;

  public SaveWorldCommandHandler(IStorageService storageService, IWorldRepository worldRepository)
  {
    _storageService = storageService;
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

    EntityMetadata entity = EntityMetadata.From(world);
    await _storageService.EnsureAvailableAsync(entity, cancellationToken);

    await _worldRepository.SaveAsync(world, cancellationToken);

    await _storageService.UpdateAsync(entity, cancellationToken);
  }
}
