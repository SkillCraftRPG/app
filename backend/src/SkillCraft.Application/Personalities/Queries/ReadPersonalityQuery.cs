using MediatR;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts.Personalities;

namespace SkillCraft.Application.Personalities.Queries;

public record ReadPersonalityQuery(Guid Id) : Activity, IRequest<PersonalityModel?>;

internal class ReadPersonalityQueryHandler : IRequestHandler<ReadPersonalityQuery, PersonalityModel?>
{
  private readonly IPermissionService _permissionService;
  private readonly IPersonalityQuerier _personalityQuerier;

  public ReadPersonalityQueryHandler(IPermissionService permissionService, IPersonalityQuerier personalityQuerier)
  {
    _permissionService = permissionService;
    _personalityQuerier = personalityQuerier;
  }

  public async Task<PersonalityModel?> Handle(ReadPersonalityQuery query, CancellationToken cancellationToken)
  {
    PersonalityModel? personality = await _personalityQuerier.ReadAsync(query.Id, cancellationToken);
    if (personality != null)
    {
      await _permissionService.EnsureCanPreviewAsync(query, personality.GetMetadata(), cancellationToken);
    }

    return personality;
  }
}
