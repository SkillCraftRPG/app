using MediatR;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Castes;

namespace SkillCraft.Application.Castes.Queries;

public record ReadCasteQuery(Guid Id) : Activity, IRequest<CasteModel?>;

internal class ReadCasteQueryHandler : IRequestHandler<ReadCasteQuery, CasteModel?>
{
  private readonly ICasteQuerier _casteQuerier;
  private readonly IPermissionService _permissionService;

  public ReadCasteQueryHandler(ICasteQuerier casteQuerier, IPermissionService permissionService)
  {
    _casteQuerier = casteQuerier;
    _permissionService = permissionService;
  }

  public async Task<CasteModel?> Handle(ReadCasteQuery query, CancellationToken cancellationToken)
  {
    await _permissionService.EnsureCanPreviewAsync(query, EntityType.Caste, cancellationToken);

    return await _casteQuerier.ReadAsync(query.GetWorldId(), query.Id, cancellationToken);
  }
}
