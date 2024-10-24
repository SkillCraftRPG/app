using MediatR;
using SkillCraft.Application.Aspects;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Characters;
using SkillCraft.Domain.Aspects;
using SkillCraft.Domain.Worlds;

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

    WorldId worldId = activity.GetWorldId();
    IEnumerable<AspectId> ids = query.Ids.Distinct().Select(id => new AspectId(worldId, id));
    IReadOnlyCollection<Aspect> aspects = await _aspectRepository.LoadAsync(ids, cancellationToken);

    IEnumerable<Guid> foundIds = aspects.Select(aspect => aspect.EntityId).Distinct();
    IEnumerable<Guid> missingIds = query.Ids.Except(foundIds).Distinct();
    if (missingIds.Any())
    {
      throw new AspectsNotFoundException(worldId, missingIds, PropertyName);
    }

    return aspects;
  }
}
