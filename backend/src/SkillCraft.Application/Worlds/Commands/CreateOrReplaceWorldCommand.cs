using FluentValidation;
using MediatR;
using SkillCraft.Application.Permissions;
using SkillCraft.Application.Storages;
using SkillCraft.Application.Worlds.Validators;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Worlds;
using SkillCraft.Domain;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Worlds.Commands;

public record CreateOrReplaceWorldResult(WorldModel? World = null, bool Created = false);

/// <exception cref="NotEnoughAvailableStorageException"></exception>
/// <exception cref="PermissionDeniedException"></exception>
/// <exception cref="SlugAlreadyUsedException"></exception>
/// <exception cref="ValidationException"></exception>
public record CreateOrReplaceWorldCommand(Guid? Id, CreateOrReplaceWorldPayload Payload, long? Version) : Activity, IRequest<CreateOrReplaceWorldResult>;

internal class CreateOrReplaceWorldCommandHandler : IRequestHandler<CreateOrReplaceWorldCommand, CreateOrReplaceWorldResult>
{
  private readonly IPermissionService _permissionService;
  private readonly ISender _sender;
  private readonly IWorldQuerier _worldQuerier;
  private readonly IWorldRepository _worldRepository;

  public CreateOrReplaceWorldCommandHandler(
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

  public async Task<CreateOrReplaceWorldResult> Handle(CreateOrReplaceWorldCommand command, CancellationToken cancellationToken)
  {
    new CreateOrReplaceWorldValidator().ValidateAndThrow(command.Payload);

    World? world = await FindAsync(command, cancellationToken);
    bool created = false;
    if (world == null)
    {
      if (command.Version.HasValue)
      {
        return new CreateOrReplaceWorldResult();
      }

      world = await CreateAsync(command, cancellationToken);
      created = true;
    }
    else
    {
      await ReplaceAsync(command, world, cancellationToken);
    }

    await _sender.Send(new SaveWorldCommand(world), cancellationToken);

    WorldModel model = await _worldQuerier.ReadAsync(world, cancellationToken);
    return new CreateOrReplaceWorldResult(model, created);
  }

  private async Task<World?> FindAsync(CreateOrReplaceWorldCommand command, CancellationToken cancellationToken)
  {
    if (!command.Id.HasValue)
    {
      return null;
    }

    WorldId id = new(command.Id.Value);
    return await _worldRepository.LoadAsync(id, cancellationToken);
  }

  private async Task<World> CreateAsync(CreateOrReplaceWorldCommand command, CancellationToken cancellationToken)
  {
    await _permissionService.EnsureCanCreateAsync(command, EntityType.World, cancellationToken);

    CreateOrReplaceWorldPayload payload = command.Payload;
    UserId userId = command.GetUserId();

    WorldId? id = command.Id.HasValue ? new(command.Id.Value) : null;
    World world = new(new Slug(payload.Slug), userId, id)
    {
      Name = Name.TryCreate(payload.Name),
      Description = Description.TryCreate(payload.Description)
    };
    world.Update(userId);

    return world;
  }

  private async Task ReplaceAsync(CreateOrReplaceWorldCommand command, World world, CancellationToken cancellationToken)
  {
    await _permissionService.EnsureCanUpdateAsync(command, world, cancellationToken);

    CreateOrReplaceWorldPayload payload = command.Payload;
    UserId userId = command.GetUserId();

    World? reference = null;
    if (command.Version.HasValue)
    {
      reference = await _worldRepository.LoadAsync(world.Id, command.Version.Value, cancellationToken);
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

    world.Update(userId);
  }
}
