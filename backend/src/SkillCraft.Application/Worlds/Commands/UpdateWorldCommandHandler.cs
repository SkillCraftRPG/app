using FluentValidation;
using Logitar.Identity.Domain.Shared;
using MediatR;
using SkillCraft.Application.Permissions;
using SkillCraft.Application.Worlds.Validators;
using SkillCraft.Contracts.Worlds;
using SkillCraft.Domain;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Worlds.Commands;

internal class UpdateWorldCommandHandler : IRequestHandler<UpdateWorldCommand, World?>
{
  private readonly IPermissionService _permissionService;
  private readonly ISender _sender;
  private readonly IWorldQuerier _worldQuerier;
  private readonly IWorldRepository _worldRepository;

  public UpdateWorldCommandHandler(IPermissionService permissionService, ISender sender, IWorldQuerier worldQuerier, IWorldRepository worldRepository)
  {
    _permissionService = permissionService;
    _sender = sender;
    _worldQuerier = worldQuerier;
    _worldRepository = worldRepository;
  }

  public async Task<World?> Handle(UpdateWorldCommand command, CancellationToken cancellationToken)
  {
    UpdateWorldPayload payload = command.Payload;
    new UpdateWorldValidator().ValidateAndThrow(payload);

    WorldId id = new(command.Id);
    WorldAggregate? world = await _worldRepository.LoadAsync(id, cancellationToken);
    if (world == null)
    {
      return null;
    }

    _permissionService.EnsureCanUpdateWorld(command.GetUser(), world);

    if (!string.IsNullOrWhiteSpace(payload.UniqueSlug))
    {
      world.UniqueSlug = new SlugUnit(payload.UniqueSlug);
    }
    if (payload.DisplayName != null)
    {
      world.DisplayName = DisplayNameUnit.TryCreate(payload.DisplayName.Value);
    }
    if (payload.Description != null)
    {
      world.Description = DescriptionUnit.TryCreate(payload.Description.Value);
    }
    world.Update(command.ActorId);

    await _sender.Send(new SaveWorldCommand(world), cancellationToken);

    return await _worldQuerier.ReadAsync(world, cancellationToken);
  }
}
