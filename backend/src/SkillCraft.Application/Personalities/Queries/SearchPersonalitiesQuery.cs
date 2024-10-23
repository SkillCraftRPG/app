using Logitar.Portal.Contracts.Search;
using MediatR;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Personalities;

namespace SkillCraft.Application.Personalities.Queries;

/// <exception cref="PermissionDeniedException"></exception>
public record SearchPersonalitiesQuery(SearchPersonalitiesPayload Payload) : Activity, IRequest<SearchResults<PersonalityModel>>;

internal class SearchPersonalitiesQueryHandler : IRequestHandler<SearchPersonalitiesQuery, SearchResults<PersonalityModel>>
{
  private readonly IPermissionService _permissionService;
  private readonly IPersonalityQuerier _personalityQuerier;

  public SearchPersonalitiesQueryHandler(IPermissionService permissionService, IPersonalityQuerier personalityQuerier)
  {
    _permissionService = permissionService;
    _personalityQuerier = personalityQuerier;
  }

  public async Task<SearchResults<PersonalityModel>> Handle(SearchPersonalitiesQuery query, CancellationToken cancellationToken)
  {
    await _permissionService.EnsureCanPreviewAsync(query, EntityType.Personality, cancellationToken);

    return await _personalityQuerier.SearchAsync(query.GetWorldId(), query.Payload, cancellationToken);
  }
}
