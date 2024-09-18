using MediatR;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts.Lineages;

namespace SkillCraft.Application.Lineages.Queries;

public record ReadLineageQuery(Guid Id) : Activity, IRequest<LineageModel?>;

internal class ReadLineageQueryHandler : IRequestHandler<ReadLineageQuery, LineageModel?>
{
  private readonly ILineageQuerier _lineageQuerier;
  private readonly IPermissionService _permissionService;

  public ReadLineageQueryHandler(ILineageQuerier lineageQuerier, IPermissionService permissionService)
  {
    _lineageQuerier = lineageQuerier;
    _permissionService = permissionService;
  }

  public async Task<LineageModel?> Handle(ReadLineageQuery query, CancellationToken cancellationToken)
  {
    LineageModel? lineage = await _lineageQuerier.ReadAsync(query.Id, cancellationToken);
    if (lineage != null)
    {
      await _permissionService.EnsureCanPreviewAsync(query, lineage.GetMetadata(), cancellationToken);
    }

    return lineage;
  }
}
