using FluentValidation;
using Logitar.Identity.Domain.Shared;
using MediatR;
using SkillCraft.Application.Permissions;
using SkillCraft.Application.Worlds.Validators;
using SkillCraft.Contracts.Worlds;
using SkillCraft.Domain;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Worlds.Commands;

internal class ReplaceWorldCommandHandler : IRequestHandler<ReplaceWorldCommand, World?>
{
  private readonly IPermissionService _permissionService;
  private readonly ISender _sender;
  private readonly IWorldQuerier _worldQuerier;
  private readonly IWorldRepository _worldRepository;

  public ReplaceWorldCommandHandler(IPermissionService permissionService, ISender sender, IWorldQuerier worldQuerier, IWorldRepository worldRepository)
  {
    _permissionService = permissionService;
    _sender = sender;
    _worldQuerier = worldQuerier;
    _worldRepository = worldRepository;
  }

  public async Task<World?> Handle(ReplaceWorldCommand command, CancellationToken cancellationToken)
  {
    ReplaceWorldPayload payload = command.Payload;
    new ReplaceWorldValidator().ValidateAndThrow(payload);

    WorldId id = new(command.Id);
    WorldAggregate? world = await _worldRepository.LoadAsync(id, cancellationToken);
    if (world == null)
    {
      return null;
    }

    _permissionService.EnsureCanUpdateWorld(command.GetUser(), world);

    WorldAggregate? reference = null;
    if (command.Version.HasValue)
    {
      reference = await _worldRepository.LoadAsync(id, command.Version.Value, cancellationToken);
    }

    SlugUnit uniqueSlug = new(payload.UniqueSlug);
    DisplayNameUnit? displayName = DisplayNameUnit.TryCreate(payload.DisplayName);
    DescriptionUnit? description = DescriptionUnit.TryCreate(payload.Description);
    if (reference == null)
    {
      world.UniqueSlug = uniqueSlug;
      world.DisplayName = displayName;
      world.Description = description;
    }
    else
    {
      if (reference.UniqueSlug != uniqueSlug)
      {
        world.UniqueSlug = uniqueSlug;
      }
      if (reference.DisplayName != displayName)
      {
        world.DisplayName = displayName;
      }
      if (reference.Description != description)
      {
        world.Description = description;
      }
    }
    world.Update(command.ActorId);

    await _sender.Send(new SaveWorldCommand(world), cancellationToken);

    return await _worldQuerier.ReadAsync(world, cancellationToken);
  }
}
