using MediatR;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts.Aspects;

namespace SkillCraft.Application.Aspects.Queries;

public record ReadAspectQuery(Guid Id) : Activity, IRequest<AspectModel?>;

internal class ReadAspectQueryHandler : IRequestHandler<ReadAspectQuery, AspectModel?>
{
  private readonly IAspectQuerier _aspectQuerier;
  private readonly IPermissionService _permissionService;

  public ReadAspectQueryHandler(IAspectQuerier aspectQuerier, IPermissionService permissionService)
  {
    _aspectQuerier = aspectQuerier;
    _permissionService = permissionService;
  }

  public async Task<AspectModel?> Handle(ReadAspectQuery query, CancellationToken cancellationToken)
  {
    AspectModel? aspect = await _aspectQuerier.ReadAsync(query.Id, cancellationToken);
    if (aspect != null)
    {
      await _permissionService.EnsureCanPreviewAsync(query, aspect.GetMetadata(), cancellationToken);
    }

    return aspect;
  }
}
