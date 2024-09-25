using Logitar.Portal.Contracts.Search;
using MediatR;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Languages;

namespace SkillCraft.Application.Languages.Queries;

public record SearchLanguagesQuery(SearchLanguagesPayload Payload) : Activity, IRequest<SearchResults<LanguageModel>>;

internal class SearchLanguagesQueryHandler : IRequestHandler<SearchLanguagesQuery, SearchResults<LanguageModel>>
{
  private readonly ILanguageQuerier _languageQuerier;
  private readonly IPermissionService _permissionService;

  public SearchLanguagesQueryHandler(ILanguageQuerier languageQuerier, IPermissionService permissionService)
  {
    _languageQuerier = languageQuerier;
    _permissionService = permissionService;
  }

  public async Task<SearchResults<LanguageModel>> Handle(SearchLanguagesQuery query, CancellationToken cancellationToken)
  {
    await _permissionService.EnsureCanPreviewAsync(query, EntityType.Language, cancellationToken);

    return await _languageQuerier.SearchAsync(query.GetWorldId(), query.Payload, cancellationToken);
  }
}
