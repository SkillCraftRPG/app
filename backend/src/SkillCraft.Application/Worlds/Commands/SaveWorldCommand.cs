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

public record SaveWorldResult(WorldModel? World = null, bool Created = false);

public record SaveWorldCommand(Guid? Id, SaveWorldPayload Payload, long? Version) : Activity, IRequest<SaveWorldResult>;

internal class SaveWorldCommandHandler : WorldCommandHandler, IRequestHandler<SaveWorldCommand, SaveWorldResult>
{
  private readonly IPermissionService _permissionService;
  private readonly IWorldQuerier _worldQuerier;
  private readonly IWorldRepository _worldRepository;

  public SaveWorldCommandHandler(
    IPermissionService permissionService,
    IStorageService storageService,
    IWorldQuerier worldQuerier,
    IWorldRepository worldRepository) : base(storageService, worldQuerier, worldRepository)
  {
    _permissionService = permissionService;
    _worldQuerier = worldQuerier;
    _worldRepository = worldRepository;
  }

  public async Task<SaveWorldResult> Handle(SaveWorldCommand command, CancellationToken cancellationToken)
  {
    new SaveWorldValidator().ValidateAndThrow(command.Payload);

    World? world = await FindAsync(command, cancellationToken);
    bool created = false;
    if (world == null)
    {
      if (command.Version.HasValue)
      {
        return new SaveWorldResult();
      }

      world = await CreateAsync(command, cancellationToken);
      created = true;
    }
    else
    {
      await ReplaceAsync(command, world, cancellationToken);
    }

    await SaveAsync(world, cancellationToken);

    WorldModel model = await _worldQuerier.ReadAsync(world, cancellationToken);
    return new SaveWorldResult(model, created);
  }

  private async Task<World?> FindAsync(SaveWorldCommand command, CancellationToken cancellationToken)
  {
    if (!command.Id.HasValue)
    {
      return null;
    }

    WorldId id = new(command.Id.Value);
    return await _worldRepository.LoadAsync(id, cancellationToken);
  }

  private async Task<World> CreateAsync(SaveWorldCommand command, CancellationToken cancellationToken)
  {
    await _permissionService.EnsureCanCreateAsync(command, EntityType.World, cancellationToken);

    SaveWorldPayload payload = command.Payload;
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

  private async Task ReplaceAsync(SaveWorldCommand command, World world, CancellationToken cancellationToken)
  {
    await _permissionService.EnsureCanUpdateAsync(command, world, cancellationToken);

    SaveWorldPayload payload = command.Payload;
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
