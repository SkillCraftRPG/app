using Logitar.Portal.Contracts;
using MediatR;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts.Worlds;

namespace SkillCraft.Application.Worlds.Queries;

/// <exception cref="PermissionDeniedException"></exception>
/// <exception cref="TooManyResultsException{T}"></exception>
public record ReadWorldQuery(Guid? Id, string? Slug) : Activity, IRequest<WorldModel?>;

internal class ReadWorldQueryHandler : IRequestHandler<ReadWorldQuery, WorldModel?>
{
  private readonly IPermissionService _permissionService;
  private readonly IWorldQuerier _worldQuerier;

  public ReadWorldQueryHandler(IPermissionService permissionService, IWorldQuerier worldQuerier)
  {
    _permissionService = permissionService;
    _worldQuerier = worldQuerier;
  }

  public async Task<WorldModel?> Handle(ReadWorldQuery query, CancellationToken cancellationToken)
  {
    Dictionary<Guid, WorldModel> worlds = new(capacity: 2);

    if (query.Id.HasValue)
    {
      WorldModel? world = await _worldQuerier.ReadAsync(query.Id.Value, cancellationToken);
      if (world != null)
      {
        await _permissionService.EnsureCanPreviewAsync(query, world, cancellationToken);

        worlds[world.Id] = world;
      }
    }

    if (!string.IsNullOrWhiteSpace(query.Slug))
    {
      WorldModel? world = await _worldQuerier.ReadAsync(query.Slug, cancellationToken);
      if (world != null)
      {
        await _permissionService.EnsureCanPreviewAsync(query, world, cancellationToken);

        worlds[world.Id] = world;
      }
    }

    if (worlds.Count > 1)
    {
      throw new TooManyResultsException<WorldModel>(expectedCount: 1, worlds.Count);
    }

    return worlds.Values.SingleOrDefault();
  }
}
