using MediatR;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Natures;

namespace SkillCraft.Application.Natures.Queries;

/// <exception cref="PermissionDeniedException"></exception>
public record ReadNatureQuery(Guid Id) : Activity, IRequest<NatureModel?>;

internal class ReadNatureQueryHandler : IRequestHandler<ReadNatureQuery, NatureModel?>
{
  private readonly INatureQuerier _natureQuerier;
  private readonly IPermissionService _permissionService;

  public ReadNatureQueryHandler(INatureQuerier natureQuerier, IPermissionService permissionService)
  {
    _natureQuerier = natureQuerier;
    _permissionService = permissionService;
  }

  public async Task<NatureModel?> Handle(ReadNatureQuery query, CancellationToken cancellationToken)
  {
    await _permissionService.EnsureCanPreviewAsync(query, EntityType.Nature, cancellationToken);

    return await _natureQuerier.ReadAsync(query.GetWorldId(), query.Id);
  }
}
