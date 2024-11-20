using Logitar.Portal.Contracts.Search;
using Moq;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Natures;

namespace SkillCraft.Application.Natures.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class SearchNaturesQueryHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<INatureQuerier> _natureQuerier = new();
  private readonly Mock<IPermissionService> _permissionService = new();

  private readonly SearchNaturesQueryHandler _handler;

  public SearchNaturesQueryHandlerTests()
  {
    _handler = new(_natureQuerier.Object, _permissionService.Object);
  }

  [Fact(DisplayName = "It should return the search results.")]
  public async Task It_should_return_the_search_results()
  {
    SearchNaturesPayload payload = new();
    SearchNaturesQuery query = new(payload);
    query.Contextualize(new WorldMock());

    SearchResults<NatureModel> results = new();
    _natureQuerier.Setup(x => x.SearchAsync(query.GetWorldId(), payload, _cancellationToken)).ReturnsAsync(results);

    SearchResults<NatureModel> natures = await _handler.Handle(query, _cancellationToken);

    Assert.Same(results, natures);

    _permissionService.Verify(x => x.EnsureCanPreviewAsync(query, EntityType.Nature, _cancellationToken), Times.Once);
  }
}
