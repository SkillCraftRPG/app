using MediatR;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;
using SkillCraft.Domain.Languages;
using SkillCraft.Domain.Worlds;
using Action = SkillCraft.Application.Permissions.Action;

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

    IEnumerable<LanguageId> ids = query.Ids.Distinct().Select(id => new LanguageId(id));
    IReadOnlyCollection<Language> languages = await _languageRepository.LoadAsync(ids, cancellationToken);

    WorldId worldId = activity.GetWorldId();
    foreach (Language language in languages)
    {
      if (language.WorldId != worldId)
      {
        throw new PermissionDeniedException(Action.Preview, EntityType.Language, activity.GetUser(), activity.GetWorld(), language.Id.ToGuid());
      }
    }

    IEnumerable<Guid> foundIds = languages.Select(language => language.Id.ToGuid()).Distinct();
    IEnumerable<Guid> missingIds = query.Ids.Except(foundIds).Distinct();
    if (missingIds.Any())
    {
      throw new LanguagesNotFoundException(missingIds, nameof(query.Ids));
    }

    return languages;
  }
}
