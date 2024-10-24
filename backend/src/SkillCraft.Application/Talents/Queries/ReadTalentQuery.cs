using MediatR;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Talents;

namespace SkillCraft.Application.Talents.Queries;

/// <exception cref="PermissionDeniedException"></exception>
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
    await _permissionService.EnsureCanPreviewAsync(query, EntityType.Talent, cancellationToken);

    return await _talentQuerier.ReadAsync(query.GetWorldId(), query.Id, cancellationToken);
  }
}
