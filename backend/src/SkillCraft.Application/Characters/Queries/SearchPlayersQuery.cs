using Logitar.Portal.Contracts.Search;
using MediatR;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;

namespace SkillCraft.Application.Characters.Queries;

/// <exception cref="PermissionDeniedException"></exception>
public record SearchPlayersQuery : Activity, IRequest<SearchResults<string>>;

internal class SearchPlayersQueryHandler : IRequestHandler<SearchPlayersQuery, SearchResults<string>>
{
  private readonly ICharacterQuerier _characterQuerier;
  private readonly IPermissionService _permissionService;

  public SearchPlayersQueryHandler(ICharacterQuerier characterQuerier, IPermissionService permissionService)
  {
    _characterQuerier = characterQuerier;
    _permissionService = permissionService;
  }

  public async Task<SearchResults<string>> Handle(SearchPlayersQuery query, CancellationToken cancellationToken)
  {
    await _permissionService.EnsureCanPreviewAsync(query, EntityType.Character, cancellationToken);

    IReadOnlyCollection<string> players = await _characterQuerier.ListPlayersAsync(query.GetWorldId(), cancellationToken);
    return new SearchResults<string>(players);
  }
}
