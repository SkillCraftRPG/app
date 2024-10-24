using Logitar.Portal.Contracts.Search;
using MediatR;
using SkillCraft.Application.Lineages;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Characters;
using SkillCraft.Contracts.Lineages;
using SkillCraft.Domain.Lineages;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Characters.Creation;

internal record ResolveLineageQuery(Activity Activity, Guid Id) : IRequest<Lineage>;

internal class ResolveLineageQueryHandler : IRequestHandler<ResolveLineageQuery, Lineage>
{
  private readonly ILineageQuerier _lineageQuerier;
  private readonly ILineageRepository _lineageRepository;
  private readonly IPermissionService _permissionService;

  public ResolveLineageQueryHandler(ILineageQuerier lineageQuerier, ILineageRepository lineageRepository, IPermissionService permissionService)
  {
    _lineageQuerier = lineageQuerier;
    _lineageRepository = lineageRepository;
    _permissionService = permissionService;
  }

  public async Task<Lineage> Handle(ResolveLineageQuery query, CancellationToken cancellationToken)
  {
    Activity activity = query.Activity;
    await _permissionService.EnsureCanPreviewAsync(activity, EntityType.Lineage, cancellationToken);

    WorldId worldId = activity.GetWorldId();
    LineageId lineageId = new(worldId, query.Id);
    Lineage lineage = await _lineageRepository.LoadAsync(lineageId, cancellationToken)
      ?? throw new LineageNotFoundException(lineageId, nameof(CreateCharacterPayload.LineageId));

    if (lineage.ParentId == null)
    {
      SearchLineagesPayload payload = new()
      {
        ParentId = lineage.EntityId,
        Limit = 1
      };
      SearchResults<LineageModel> results = await _lineageQuerier.SearchAsync(worldId, payload, cancellationToken);
      if (results.Total > 0)
      {
        throw new InvalidCharacterLineageException(lineage, nameof(CreateCharacterPayload.LineageId));
      }
    }

    return lineage;
  }
}
