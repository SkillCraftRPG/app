using Logitar.Portal.Contracts.Search;
using MediatR;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;

namespace SkillCraft.Application.Languages.Queries;

/// <exception cref="PermissionDeniedException"></exception>
public record SearchScriptsQuery : Activity, IRequest<SearchResults<string>>;

internal class SearchScriptsQueryHandler : IRequestHandler<SearchScriptsQuery, SearchResults<string>>
{
  private readonly ILanguageQuerier _languageQuerier;
  private readonly IPermissionService _permissionService;

  public SearchScriptsQueryHandler(ILanguageQuerier languageQuerier, IPermissionService permissionService)
  {
    _languageQuerier = languageQuerier;
    _permissionService = permissionService;
  }

  public async Task<SearchResults<string>> Handle(SearchScriptsQuery query, CancellationToken cancellationToken)
  {
    await _permissionService.EnsureCanPreviewAsync(query, EntityType.Language, cancellationToken);

    IReadOnlyCollection<string> scripts = await _languageQuerier.ListScriptsAsync(query.GetWorldId(), cancellationToken);
    return new SearchResults<string>(scripts);
  }
}
