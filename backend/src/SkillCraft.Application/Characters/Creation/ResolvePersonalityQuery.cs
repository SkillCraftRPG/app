using MediatR;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Characters;
using SkillCraft.Domain.Personalities;

namespace SkillCraft.Application.Characters.Creation;

internal record ResolvePersonalityQuery(Activity Activity, Guid Id) : IRequest<Personality>;

internal class ResolvePersonalityQueryHandler : IRequestHandler<ResolvePersonalityQuery, Personality>
{
  private readonly IPermissionService _permissionService;
  private readonly IPersonalityRepository _personalityRepository;

  public ResolvePersonalityQueryHandler(IPermissionService permissionService, IPersonalityRepository personalityRepository)
  {
    _permissionService = permissionService;
    _personalityRepository = personalityRepository;
  }

  public async Task<Personality> Handle(ResolvePersonalityQuery query, CancellationToken cancellationToken)
  {
    Activity activity = query.Activity;
    await _permissionService.EnsureCanPreviewAsync(activity, EntityType.Personality, cancellationToken);

    PersonalityId id = new(activity.GetWorldId(), query.Id);
    Personality personality = await _personalityRepository.LoadAsync(id, cancellationToken)
      ?? throw new AggregateNotFoundException<Personality>(id.AggregateId, nameof(CreateCharacterPayload.PersonalityId));

    return personality;
  }
}
