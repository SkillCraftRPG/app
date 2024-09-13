using MediatR;
using SkillCraft.Application.Permissions;
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
    CasteModel? caste = await _casteQuerier.ReadAsync(query.Id, cancellationToken);
    if (caste != null)
    {
      await _permissionService.EnsureCanPreviewAsync(query, EntityMetadata.From(caste), cancellationToken);
    }

    return caste;
  }
}
