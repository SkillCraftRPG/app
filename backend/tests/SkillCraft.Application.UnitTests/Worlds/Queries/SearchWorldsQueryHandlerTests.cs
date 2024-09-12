using Logitar.Portal.Contracts.Search;
using Moq;
using SkillCraft.Contracts.Worlds;

namespace SkillCraft.Application.Worlds.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class SearchWorldsQueryHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IWorldQuerier> _worldQuerier = new();

  private readonly SearchWorldsQueryHandler _handler;

  public SearchWorldsQueryHandlerTests()
  {
    _handler = new(_worldQuerier.Object);
  }

  [Fact(DisplayName = "It should return the search results.")]
  public async Task It_should_return_the_search_results()
  {
    SearchWorldsPayload payload = new();
    SearchWorldsQuery query = new(payload);
    query.Contextualize();

    SearchResults<WorldModel> results = new();
    _worldQuerier.Setup(x => x.SearchAsync(query.GetUser(), payload, _cancellationToken)).ReturnsAsync(results);

    SearchResults<WorldModel> worlds = await _handler.Handle(query, _cancellationToken);

    Assert.Same(results, worlds);
  }
}
