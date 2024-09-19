using MediatR;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts.Talents;

namespace SkillCraft.Application.Talents.Queries;

public record ReadTalentQuery(Guid Id) : Activity, IRequest<TalentModel?>;

internal class ReadTalentQueryHandler : IRequestHandler<ReadTalentQuery, TalentModel?>
{
  private readonly IPermissionService _permissionService;
  private readonly ITalentQuerier _talentQuerier;

  public ReadTalentQueryHandler(IPermissionService permissionService, ITalentQuerier talentQuerier)
  {
    _permissionService = permissionService;
    _talentQuerier = talentQuerier;
  }

  public async Task<TalentModel?> Handle(ReadTalentQuery query, CancellationToken cancellationToken)
  {
    TalentModel? talent = await _talentQuerier.ReadAsync(query.Id, cancellationToken);
    if (talent != null)
    {
      await _permissionService.EnsureCanPreviewAsync(query, talent.GetMetadata(), cancellationToken);
    }

    return talent;
  }
}
