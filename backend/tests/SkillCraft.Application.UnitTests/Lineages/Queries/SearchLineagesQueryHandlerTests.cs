using Logitar.Portal.Contracts.Search;
using Moq;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts.Lineages;

namespace SkillCraft.Application.Lineages.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class SearchLineagesQueryHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<ILineageQuerier> _lineageQuerier = new();
  private readonly Mock<IPermissionService> _permissionService = new();

  private readonly SearchLineagesQueryHandler _handler;

  public SearchLineagesQueryHandlerTests()
  {
    _handler = new(_lineageQuerier.Object, _permissionService.Object);
  }

  [Fact(DisplayName = "It should return the search results.")]
  public async Task It_should_return_the_search_results()
  {
    SearchLineagesPayload payload = new();
    SearchLineagesQuery query = new(payload);
    query.Contextualize(new WorldMock());

    SearchResults<LineageModel> results = new();
    _lineageQuerier.Setup(x => x.SearchAsync(query.GetWorldId(), payload, _cancellationToken)).ReturnsAsync(results);

    SearchResults<LineageModel> lineages = await _handler.Handle(query, _cancellationToken);

    Assert.Same(results, lineages);
  }
}
