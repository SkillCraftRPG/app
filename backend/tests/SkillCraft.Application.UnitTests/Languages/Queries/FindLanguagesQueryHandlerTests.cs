using Moq;
using SkillCraft.Domain;
using SkillCraft.Domain.Languages;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Languages.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class FindLanguagesQueryHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<ILanguageRepository> _languageRepository = new();

  private readonly FindLanguagesQueryHandler _handler;

  private readonly World _world;
  private readonly Language _language;

  public FindLanguagesQueryHandlerTests()
  {
    _handler = new(_languageRepository.Object);

    _world = new(new Slug("ungar"), UserId.NewId());
    _language = new(_world.Id, new Name("Commun"), _world.OwnerId);
  }

  [Fact(DisplayName = "It should return prematurely when there is no ID.")]
  public async Task It_should_return_prematurely_when_there_is_no_Id()
  {
    FindLanguagesQuery query = new([]);

    IReadOnlyCollection<Language> languages = await _handler.Handle(query, _cancellationToken);
    Assert.Empty(languages);

    _languageRepository.Verify(x => x.LoadAsync(It.IsAny<IEnumerable<LanguageId>>(), It.IsAny<CancellationToken>()), Times.Never);
  }

  [Fact(DisplayName = "It should return the languages found.")]
  public async Task It_should_return_the_languages_found()
  {
    _languageRepository.Setup(x => x.LoadAsync(It.Is<IEnumerable<LanguageId>>(y => y.Single() == _language.Id), _cancellationToken))
      .ReturnsAsync([_language]);

    FindLanguagesQuery query = new([_language.Id.ToGuid()]);
    IReadOnlyCollection<Language> languages = await _handler.Handle(query, _cancellationToken);
    Assert.Same(_language, Assert.Single(languages));
  }

  [Fact(DisplayName = "It should throw LanguagesNotFoundException when some languages could not be found.")]
  public async Task It_should_throw_LanguagesNotFoundException_when_some_languages_could_not_be_found()
  {
    FindLanguagesQuery query = new([_language.Id.ToGuid(), Guid.NewGuid(), Guid.Empty]);
    _languageRepository.Setup(x => x.LoadAsync(It.Is<IEnumerable<LanguageId>>(y => y.Count() == 3
      && query.Ids.All(id => y.Contains(new LanguageId(id)))
    ), _cancellationToken)).ReturnsAsync([_language]);

    var exception = await Assert.ThrowsAsync<LanguagesNotFoundException>(async () => await _handler.Handle(query, _cancellationToken));
    Assert.Equal(query.Ids.Except([_language.Id.ToGuid()]), exception.Ids);
    Assert.Equal("Ids", exception.PropertyName);
  }
}
