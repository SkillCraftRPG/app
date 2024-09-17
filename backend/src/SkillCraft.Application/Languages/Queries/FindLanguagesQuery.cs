using MediatR;
using SkillCraft.Domain.Languages;

namespace SkillCraft.Application.Languages.Queries;

internal record FindLanguagesQuery(IEnumerable<Guid> Ids) : IRequest<IReadOnlyCollection<Language>>;

internal class FindLanguagesQueryHandler : IRequestHandler<FindLanguagesQuery, IReadOnlyCollection<Language>>
{
  private readonly ILanguageRepository _languageRepository;

  public FindLanguagesQueryHandler(ILanguageRepository languageRepository)
  {
    _languageRepository = languageRepository;
  }

  public async Task<IReadOnlyCollection<Language>> Handle(FindLanguagesQuery query, CancellationToken cancellationToken)
  {
    if (!query.Ids.Any())
    {
      return [];
    }

    IEnumerable<LanguageId> ids = query.Ids.Select(id => new LanguageId(id)).Distinct();
    IReadOnlyCollection<Language> languages = await _languageRepository.LoadAsync(ids, cancellationToken);

    IEnumerable<Guid> foundIds = languages.Select(language => language.Id.ToGuid()).Distinct();
    IEnumerable<Guid> missingIds = query.Ids.Except(foundIds).Distinct();
    if (missingIds.Any())
    {
      throw new NotImplementedException(); // TODO(fpion): typed exception
    }

    return languages;
  }
}
