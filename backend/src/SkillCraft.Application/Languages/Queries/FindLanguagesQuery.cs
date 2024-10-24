using MediatR;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;
using SkillCraft.Domain.Languages;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Languages.Queries;

internal record FindLanguagesQuery(Activity Activity, IEnumerable<Guid> Ids) : IRequest<IReadOnlyCollection<Language>>;

internal class FindLanguagesQueryHandler : IRequestHandler<FindLanguagesQuery, IReadOnlyCollection<Language>>
{
  private readonly ILanguageRepository _languageRepository;
  private readonly IPermissionService _permissionService;

  public FindLanguagesQueryHandler(ILanguageRepository languageRepository, IPermissionService permissionService)
  {
    _languageRepository = languageRepository;
    _permissionService = permissionService;
  }

  public async Task<IReadOnlyCollection<Language>> Handle(FindLanguagesQuery query, CancellationToken cancellationToken)
  {
    if (!query.Ids.Any())
    {
      return [];
    }

    Activity activity = query.Activity;
    await _permissionService.EnsureCanPreviewAsync(activity, EntityType.Language, cancellationToken);

    WorldId worldId = activity.GetWorldId();
    IEnumerable<LanguageId> ids = query.Ids.Distinct().Select(id => new LanguageId(worldId, id));
    IReadOnlyCollection<Language> languages = await _languageRepository.LoadAsync(ids, cancellationToken);

    IEnumerable<Guid> foundIds = languages.Select(language => language.EntityId).Distinct();
    IEnumerable<Guid> missingIds = query.Ids.Except(foundIds).Distinct();
    if (missingIds.Any())
    {
      throw new LanguagesNotFoundException(worldId, missingIds, nameof(query.Ids));
    }

    return languages;
  }
}
