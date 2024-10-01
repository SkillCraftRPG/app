using Logitar.EventSourcing;
using SkillCraft.Application.Storages;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Worlds.Commands;

internal abstract class WorldCommandHandler
{
  private readonly IStorageService _storageService;
  private readonly IWorldQuerier _worldQuerier;
  private readonly IWorldRepository _worldRepository;

  protected WorldCommandHandler(IStorageService storageService, IWorldQuerier worldQuerier, IWorldRepository worldRepository)
  {
    _storageService = storageService;
    _worldQuerier = worldQuerier;
    _worldRepository = worldRepository;
  }

  protected async Task SaveAsync(World world, CancellationToken cancellationToken)
  {
    bool hasSlugChanged = false;
    foreach (DomainEvent change in world.Changes)
    {
      if (change is World.CreatedEvent || (change is World.UpdatedEvent updatedEvent && updatedEvent.Slug != null))
      {
        hasSlugChanged = true;
        break;
      }
    }
    if (hasSlugChanged)
    {
      WorldId? otherId = await _worldQuerier.FindIdAsync(world.Slug, cancellationToken);
      if (otherId.HasValue && otherId.Value != world.Id)
      {
        throw new SlugAlreadyUsedException(world, otherId.Value, nameof(World.Slug));
      }
    }

    await _storageService.EnsureAvailableAsync(world, cancellationToken);

    await _worldRepository.SaveAsync(world, cancellationToken);

    await _storageService.UpdateAsync(world, cancellationToken);
  }
}
