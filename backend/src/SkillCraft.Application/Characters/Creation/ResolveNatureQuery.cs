using MediatR;
using SkillCraft.Application.Natures;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Characters;
using SkillCraft.Domain.Natures;

namespace SkillCraft.Application.Characters.Creation;

internal record ResolveNatureQuery(Activity Activity, Guid Id) : IRequest<Nature>;

internal class ResolveNatureQueryHandler : IRequestHandler<ResolveNatureQuery, Nature>
{
  private readonly INatureRepository _natureRepository;
  private readonly IPermissionService _permissionService;

  public ResolveNatureQueryHandler(INatureRepository natureRepository, IPermissionService permissionService)
  {
    _natureRepository = natureRepository;
    _permissionService = permissionService;
  }

  public async Task<Nature> Handle(ResolveNatureQuery query, CancellationToken cancellationToken)
  {
    Activity activity = query.Activity;
    await _permissionService.EnsureCanPreviewAsync(activity, EntityType.Nature, cancellationToken);

    NatureId id = new(activity.GetWorldId(), query.Id);
    Nature nature = await _natureRepository.LoadAsync(id, cancellationToken)
      ?? throw new NatureNotFoundException(id, nameof(CreateCharacterPayload.NatureId));

    return nature;
  }
}
