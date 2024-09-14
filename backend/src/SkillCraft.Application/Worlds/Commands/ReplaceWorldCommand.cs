using FluentValidation;
using MediatR;
using SkillCraft.Application.Permissions;
using SkillCraft.Application.Worlds.Validators;
using SkillCraft.Contracts.Worlds;
using SkillCraft.Domain;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Worlds.Commands;

public record ReplaceWorldCommand(Guid Id, ReplaceWorldPayload Payload, long? Version) : Activity, IRequest<WorldModel?>;

internal class ReplaceWorldCommandHandler : IRequestHandler<ReplaceWorldCommand, WorldModel?>
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

  public async Task<WorldModel?> Handle(ReplaceWorldCommand command, CancellationToken cancellationToken)
  {
    ReplaceWorldPayload payload = command.Payload;
    new ReplaceWorldValidator().ValidateAndThrow(payload);

    WorldId id = new(command.Id);
    World? world = await _worldRepository.LoadAsync(id, cancellationToken);
    if (world == null)
    {
      return null;
    }

    await _permissionService.EnsureCanUpdateAsync(command, world, cancellationToken);

    World? reference = null;
    if (command.Version.HasValue)
    {
      reference = await _worldRepository.LoadAsync(id, command.Version.Value, cancellationToken);
    }
    reference ??= world;

    Slug slug = new(payload.Slug);
    if (slug != reference.Slug)
    {
      world.Slug = slug;
    }
    Name? name = Name.TryCreate(payload.Name);
    if (name != reference.Name)
    {
      world.Name = name;
    }
    Description? description = Description.TryCreate(payload.Description);
    if (description != reference.Description)
    {
      world.Description = description;
    }

    world.Update(command.GetUserId());
    await _sender.Send(new SaveWorldCommand(world), cancellationToken);

    return await _worldQuerier.ReadAsync(world, cancellationToken);
  }
}
