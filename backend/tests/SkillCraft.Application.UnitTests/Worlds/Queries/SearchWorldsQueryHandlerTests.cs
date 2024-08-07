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

  [Fact(DisplayName = "It should return the correct search results.")]
  public async Task It_should_return_the_correct_search_results()
  {
    SearchWorldsPayload payload = new();

    UserMock user = new();
    SearchResults<World> worlds = new();
    _worldQuerier.Setup(x => x.SearchAsync(user, payload, _cancellationToken)).ReturnsAsync(worlds);

    SearchWorldsQuery query = new(payload);
    query.Contextualize(new ActivityContextMock(user));
    SearchResults<World> results = await _handler.Handle(query, _cancellationToken);

    Assert.Same(worlds, results);
  }
}
