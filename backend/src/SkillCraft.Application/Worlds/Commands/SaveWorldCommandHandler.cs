﻿using Logitar.EventSourcing;
using MediatR;
using SkillCraft.Application.Storage;
using SkillCraft.Domain.Worlds;
using SkillCraft.Domain.Worlds.Events;

namespace SkillCraft.Application.Worlds.Commands;

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
    WorldAggregate world = command.World;

    bool hasUniqueSlugChanged = false;
    foreach (DomainEvent change in world.Changes)
    {
      if (change is WorldCreatedEvent || (change is WorldUpdatedEvent updated && updated.UniqueSlug != null))
      {
        hasUniqueSlugChanged = true;
        break;
      }
    }

    if (hasUniqueSlugChanged)
    {
      WorldAggregate? other = await _worldRepository.LoadAsync(world.UniqueSlug, cancellationToken);
      if (other != null && !other.Equals(world))
      {
        throw new UniqueSlugAlreadyUsedException(world.UniqueSlug, nameof(world.UniqueSlug));
      }
    }

    await _storageService.EnsureAvailableAsync(world, cancellationToken);

    await _worldRepository.SaveAsync(world, cancellationToken);

    await _storageService.UpdateAsync(world, cancellationToken);
  }
}
