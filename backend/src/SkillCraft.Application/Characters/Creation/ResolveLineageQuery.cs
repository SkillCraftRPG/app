using Logitar.Portal.Contracts.Search;
using MediatR;
using SkillCraft.Application.Lineages;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts.Characters;
using SkillCraft.Contracts.Lineages;
using SkillCraft.Domain.Lineages;

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
    LineageId lineageId = new(query.Id);
    Lineage lineage = await _lineageRepository.LoadAsync(lineageId, cancellationToken)
      ?? throw new AggregateNotFoundException<Lineage>(lineageId.AggregateId, nameof(CreateCharacterPayload.LineageId));

    Activity activity = query.Activity;
    await _permissionService.EnsureCanPreviewAsync(activity, lineage.GetMetadata(), cancellationToken);

    if (lineage.ParentId == null)
    {
      SearchLineagesPayload payload = new()
      {
        ParentId = lineage.Id.ToGuid(),
        Limit = 1
      };
      SearchResults<LineageModel> results = await _lineageQuerier.SearchAsync(activity.GetWorldId(), payload, cancellationToken);
      if (results.Total > 0)
      {
        throw new InvalidCharacterLineageException(lineage, nameof(CreateCharacterPayload.LineageId));
      }
    }

    return lineage;
  }
}
