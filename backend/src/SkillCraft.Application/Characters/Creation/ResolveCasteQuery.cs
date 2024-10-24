using MediatR;
using SkillCraft.Application.Castes;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Characters;
using SkillCraft.Domain.Castes;

namespace SkillCraft.Application.Characters.Creation;

internal record ResolveCasteQuery(Activity Activity, Guid Id) : IRequest<Caste>;

internal class ResolveCasteQueryHandler : IRequestHandler<ResolveCasteQuery, Caste>
{
  private readonly ICasteRepository _casteRepository;
  private readonly IPermissionService _permissionService;

  public ResolveCasteQueryHandler(ICasteRepository casteRepository, IPermissionService permissionService)
  {
    _casteRepository = casteRepository;
    _permissionService = permissionService;
  }

  public async Task<Caste> Handle(ResolveCasteQuery query, CancellationToken cancellationToken)
  {
    Activity activity = query.Activity;
    await _permissionService.EnsureCanPreviewAsync(activity, EntityType.Caste, cancellationToken);

    CasteId id = new(activity.GetWorldId(), query.Id);
    Caste caste = await _casteRepository.LoadAsync(id, cancellationToken)
      ?? throw new CasteNotFoundException(id, nameof(CreateCharacterPayload.CasteId));

    return caste;
  }
}
