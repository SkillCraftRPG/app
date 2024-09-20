using MediatR;
using SkillCraft.Application.Permissions;
using SkillCraft.Application.Personalities;
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
    PersonalityId id = new(query.Id);
    Personality personality = await _personalityRepository.LoadAsync(id, cancellationToken)
      ?? throw new AggregateNotFoundException<Personality>(id.AggregateId, nameof(CreateCharacterPayload.PersonalityId));

    await _permissionService.EnsureCanPreviewAsync(query.Activity, personality.GetMetadata(), cancellationToken);

    return personality;
  }
}
