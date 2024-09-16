using Logitar.Portal.Contracts.Search;
using Moq;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts.Personalities;

namespace SkillCraft.Application.Personalities.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class SearchPersonalitiesQueryHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<IPersonalityQuerier> _personalityQuerier = new();

  private readonly SearchPersonalitiesQueryHandler _handler;

  public SearchPersonalitiesQueryHandlerTests()
  {
    _handler = new(_permissionService.Object, _personalityQuerier.Object);
  }

  [Fact(DisplayName = "It should return the search results.")]
  public async Task It_should_return_the_search_results()
  {
    SearchPersonalitiesPayload payload = new();
    SearchPersonalitiesQuery query = new(payload);
    query.Contextualize(new WorldMock());

    SearchResults<PersonalityModel> results = new();
    _personalityQuerier.Setup(x => x.SearchAsync(query.GetWorldId(), payload, _cancellationToken)).ReturnsAsync(results);

    SearchResults<PersonalityModel> personalities = await _handler.Handle(query, _cancellationToken);

    Assert.Same(results, personalities);
  }
}
