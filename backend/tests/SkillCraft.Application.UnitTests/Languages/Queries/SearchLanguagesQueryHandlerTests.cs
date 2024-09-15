using Logitar.Portal.Contracts.Search;
using Moq;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts.Languages;

namespace SkillCraft.Application.Languages.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class SearchLanguagesQueryHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<ILanguageQuerier> _languageQuerier = new();
  private readonly Mock<IPermissionService> _permissionService = new();

  private readonly SearchLanguagesQueryHandler _handler;

  public SearchLanguagesQueryHandlerTests()
  {
    _handler = new(_languageQuerier.Object, _permissionService.Object);
  }

  [Fact(DisplayName = "It should return the search results.")]
  public async Task It_should_return_the_search_results()
  {
    SearchLanguagesPayload payload = new();
    SearchLanguagesQuery query = new(payload);
    query.Contextualize(new WorldMock());

    SearchResults<LanguageModel> results = new();
    _languageQuerier.Setup(x => x.SearchAsync(query.GetWorldId(), payload, _cancellationToken)).ReturnsAsync(results);

    SearchResults<LanguageModel> languages = await _handler.Handle(query, _cancellationToken);

    Assert.Same(results, languages);
  }
}
