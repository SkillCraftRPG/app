using MediatR;
using SkillCraft.Application.Aspects;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts.Characters;
using SkillCraft.Domain;
using SkillCraft.Domain.Aspects;
using SkillCraft.Domain.Worlds;
using Action = SkillCraft.Application.Permissions.Action;

namespace SkillCraft.Application.Characters.Creation;

internal record ResolveAspectsQuery(Activity Activity, IEnumerable<Guid> Ids) : IRequest<IReadOnlyCollection<Aspect>>;

internal class ResolveAspectsQueryHandler : IRequestHandler<ResolveAspectsQuery, IReadOnlyCollection<Aspect>>
{
  private const string PropertyName = nameof(CreateCharacterPayload.AspectIds);

  private readonly IAspectRepository _aspectRepository;
  private readonly IPermissionService _permissionService;

  public ResolveAspectsQueryHandler(IAspectRepository aspectRepository, IPermissionService permissionService)
  {
    _aspectRepository = aspectRepository;
    _permissionService = permissionService;
  }

  public async Task<IReadOnlyCollection<Aspect>> Handle(ResolveAspectsQuery query, CancellationToken cancellationToken)
  {
    if (!query.Ids.Any())
    {
      return [];
    }

    Activity activity = query.Activity;
    await _permissionService.EnsureCanPreviewAsync(activity, EntityType.Aspect, cancellationToken);

    IEnumerable<AspectId> ids = query.Ids.Distinct().Select(id => new AspectId(id));
    IReadOnlyCollection<Aspect> aspects = await _aspectRepository.LoadAsync(ids, cancellationToken);

    WorldId worldId = activity.GetWorldId();
    foreach (Aspect aspect in aspects)
    {
      if (aspect.WorldId != worldId)
      {
        throw new PermissionDeniedException(Action.Preview, EntityType.Aspect, activity.GetUser(), activity.GetWorld(), aspect.Id.ToGuid());
      }
    }

    IEnumerable<Guid> foundIds = aspects.Select(aspect => aspect.Id.ToGuid()).Distinct();
    IEnumerable<Guid> missingIds = query.Ids.Except(foundIds).Distinct();
    if (missingIds.Any())
    {
      throw new AspectsNotFoundException(missingIds, PropertyName);
    }

    return aspects;
  }
}
