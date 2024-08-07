using MediatR;
using SkillCraft.Application.Permissions;
using SkillCraft.Application.Storage;
using SkillCraft.Contracts.Worlds;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Worlds.Commands;

internal class DeleteWorldCommandHandler : IRequestHandler<DeleteWorldCommand, World?>
{
  private readonly IPermissionService _permissionService;
  private readonly IStorageService _storageService;
  private readonly IWorldQuerier _worldQuerier;
  private readonly IWorldRepository _worldRepository;

  public DeleteWorldCommandHandler(IPermissionService permissionService, IStorageService storageService, IWorldQuerier worldQuerier, IWorldRepository worldRepository)
  {
    _permissionService = permissionService;
    _storageService = storageService;
    _worldQuerier = worldQuerier;
    _worldRepository = worldRepository;
  }

  public async Task<World?> Handle(DeleteWorldCommand command, CancellationToken cancellationToken)
  {
    WorldId id = new(command.Id);
    WorldAggregate? world = await _worldRepository.LoadAsync(id, cancellationToken);
    if (world == null)
    {
      return null;
    }

    _permissionService.EnsureCanDeleteWorld(command.GetUser(), world);
    World result = await _worldQuerier.ReadAsync(world, cancellationToken);

    world.Delete(command.ActorId);

    await _worldRepository.SaveAsync(world, cancellationToken);

    await _storageService.DeleteAsync(world, cancellationToken);

    return result;
  }
}
