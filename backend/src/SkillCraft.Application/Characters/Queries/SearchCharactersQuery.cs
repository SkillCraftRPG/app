using Logitar.Portal.Contracts.Search;
using MediatR;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Characters;

namespace SkillCraft.Application.Characters.Queries;

/// <exception cref="PermissionDeniedException"></exception>
public record SearchCharactersQuery(SearchCharactersPayload Payload) : Activity, IRequest<SearchResults<CharacterModel>>;

internal class SearchCharactersQueryHandler : IRequestHandler<SearchCharactersQuery, SearchResults<CharacterModel>>
{
  private readonly ICharacterQuerier _characterQuerier;
  private readonly IPermissionService _permissionService;

  public SearchCharactersQueryHandler(ICharacterQuerier characterQuerier, IPermissionService permissionService)
  {
    _characterQuerier = characterQuerier;
    _permissionService = permissionService;
  }

  public async Task<SearchResults<CharacterModel>> Handle(SearchCharactersQuery query, CancellationToken cancellationToken)
  {
    await _permissionService.EnsureCanPreviewAsync(query, EntityType.Character, cancellationToken);

    return await _characterQuerier.SearchAsync(query.GetWorldId(), query.Payload, cancellationToken);
  }
}
