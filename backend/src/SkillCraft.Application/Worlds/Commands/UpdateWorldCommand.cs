using FluentValidation;
using MediatR;
using SkillCraft.Application.Permissions;
using SkillCraft.Application.Worlds.Validators;
using SkillCraft.Contracts.Worlds;
using SkillCraft.Domain;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Worlds.Commands;

public record UpdateWorldCommand(Guid Id, UpdateWorldPayload Payload) : Activity, IRequest<WorldModel?>;

internal class UpdateWorldCommandHandler : IRequestHandler<UpdateWorldCommand, WorldModel?>
{
  private readonly IPermissionService _permissionService;
  private readonly ISender _sender;
  private readonly IWorldQuerier _worldQuerier;
  private readonly IWorldRepository _worldRepository;

  public UpdateWorldCommandHandler(
    IPermissionService permissionService,
    ISender sender,
    IWorldQuerier worldQuerier,
    IWorldRepository worldRepository)
  {
    _permissionService = permissionService;
    _sender = sender;
    _worldQuerier = worldQuerier;
    _worldRepository = worldRepository;
  }

  public async Task<WorldModel?> Handle(UpdateWorldCommand command, CancellationToken cancellationToken)
  {
    UpdateWorldPayload payload = command.Payload;
    new UpdateWorldValidator().ValidateAndThrow(payload);

    WorldId id = new(command.Id);
    World? world = await _worldRepository.LoadAsync(id, cancellationToken);
    if (world == null)
    {
      return null;
    }

    await _permissionService.EnsureCanUpdateAsync(command, world, cancellationToken);

    if (!string.IsNullOrWhiteSpace(payload.Slug))
    {
      world.Slug = new Slug(payload.Slug);
    }
    if (payload.Name != null)
    {
      world.Name = Name.TryCreate(payload.Name.Value);
    }
    if (payload.Description != null)
    {
      world.Description = Description.TryCreate(payload.Description.Value);
    }

    world.Update(command.GetUserId());

    await _sender.Send(new SaveWorldCommand(world), cancellationToken);

    return await _worldQuerier.ReadAsync(world, cancellationToken);
  }
}
