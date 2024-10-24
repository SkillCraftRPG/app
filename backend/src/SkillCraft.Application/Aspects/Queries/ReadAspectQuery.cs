using MediatR;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Aspects;

namespace SkillCraft.Application.Aspects.Queries;

/// <exception cref="PermissionDeniedException"></exception>
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
    await _permissionService.EnsureCanPreviewAsync(query, EntityType.Aspect, cancellationToken);

    return await _aspectQuerier.ReadAsync(query.GetWorldId(), query.Id, cancellationToken);
  }
}
