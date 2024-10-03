using MediatR;
using SkillCraft.Application.Languages;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Characters;
using SkillCraft.Domain.Languages;
using SkillCraft.Domain.Lineages;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Characters.Creation;

internal record ResolveLanguagesQuery(Activity Activity, Lineage Lineage, Lineage? Parent, IEnumerable<Guid> Ids) : IRequest<IReadOnlyCollection<Language>>;

internal class ResolveLanguagesQueryHandler : IRequestHandler<ResolveLanguagesQuery, IReadOnlyCollection<Language>>
{
  private const string PropertyName = nameof(CreateCharacterPayload.LanguageIds);

  private readonly ILanguageRepository _languageRepository;
  private readonly IPermissionService _permissionService;

  public ResolveLanguagesQueryHandler(ILanguageRepository languageRepository, IPermissionService permissionService)
  {
    _languageRepository = languageRepository;
    _permissionService = permissionService;
  }

  public async Task<IReadOnlyCollection<Language>> Handle(ResolveLanguagesQuery query, CancellationToken cancellationToken)
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
      throw new LanguagesNotFoundException(missingIds, PropertyName);
    }

    Lineage lineage = query.Lineage;
    Lineage? parent = query.Parent;
    int extra = lineage.Languages.Extra + (parent?.Languages.Extra ?? 0);
    if (languages.Count != extra)
    {
      throw new InvalidExtraLanguagesException(query.Ids, extra, PropertyName);
    }
    HashSet<LanguageId> lineageLanguages = [.. lineage.Languages.Ids, .. parent?.Languages.Ids ?? []];
    HashSet<Guid> conflictIds = new(capacity: languages.Count);
    foreach (Language language in languages)
    {
      if (lineageLanguages.Contains(language.Id))
      {
        conflictIds.Add(language.EntityId);
      }
    }
    if (conflictIds.Count > 0)
    {
      throw new LanguagesCannotIncludeLineageLanguageException(conflictIds, PropertyName);
    }

    return languages;
  }
}
