using Logitar.Portal.Contracts.Search;
using Moq;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts.Aspects;

namespace SkillCraft.Application.Aspects.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class SearchAspectsQueryHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IAspectQuerier> _aspectQuerier = new();
  private readonly Mock<IPermissionService> _permissionService = new();

  private readonly SearchAspectsQueryHandler _handler;

  public SearchAspectsQueryHandlerTests()
  {
    _handler = new(_aspectQuerier.Object, _permissionService.Object);
  }

  [Fact(DisplayName = "It should return the search results.")]
  public async Task It_should_return_the_search_results()
  {
    SearchAspectsPayload payload = new();
    SearchAspectsQuery query = new(payload);
    query.Contextualize(new WorldMock());

    SearchResults<AspectModel> results = new();
    _aspectQuerier.Setup(x => x.SearchAsync(query.GetWorldId(), payload, _cancellationToken)).ReturnsAsync(results);

    SearchResults<AspectModel> aspects = await _handler.Handle(query, _cancellationToken);

    Assert.Same(results, aspects);
  }
}
