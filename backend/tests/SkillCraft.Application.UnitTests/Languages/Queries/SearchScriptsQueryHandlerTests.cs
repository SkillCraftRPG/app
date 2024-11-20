using Logitar.Portal.Contracts.Search;
using Moq;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;

namespace SkillCraft.Application.Languages.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class SearchScriptsQueryHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<ILanguageQuerier> _languageQuerier = new();
  private readonly Mock<IPermissionService> _permissionService = new();

  private readonly SearchScriptsQueryHandler _handler;

  public SearchScriptsQueryHandlerTests()
  {
    _handler = new(_languageQuerier.Object, _permissionService.Object);
  }

  [Fact(DisplayName = "It should list the scripts.")]
  public async Task It_should_list_the_scripts()
  {
    SearchScriptsQuery query = new();
    query.Contextualize(new WorldMock());

    string[] results = ["Orrinique", "Rénon"];
    _languageQuerier.Setup(x => x.ListScriptsAsync(query.GetWorldId(), _cancellationToken)).ReturnsAsync(results);

    SearchResults<string> scripts = await _handler.Handle(query, _cancellationToken);

    Assert.Equal(results, scripts.Items);
    Assert.Equal(results.Length, scripts.Total);

    _permissionService.Verify(x => x.EnsureCanPreviewAsync(query, EntityType.Language, _cancellationToken), Times.Once);
  }
}
