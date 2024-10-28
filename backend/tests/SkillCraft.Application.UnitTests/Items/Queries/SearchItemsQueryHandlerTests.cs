using Logitar.Portal.Contracts.Search;
using Moq;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts.Items;

namespace SkillCraft.Application.Items.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class SearchItemsQueryHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IItemQuerier> _itemQuerier = new();
  private readonly Mock<IPermissionService> _permissionService = new();

  private readonly SearchItemsQueryHandler _handler;

  public SearchItemsQueryHandlerTests()
  {
    _handler = new(_itemQuerier.Object, _permissionService.Object);
  }

  [Fact(DisplayName = "It should return the search results.")]
  public async Task It_should_return_the_search_results()
  {
    SearchItemsPayload payload = new();
    SearchItemsQuery query = new(payload);
    query.Contextualize(new WorldMock());

    SearchResults<ItemModel> results = new();
    _itemQuerier.Setup(x => x.SearchAsync(query.GetWorldId(), payload, _cancellationToken)).ReturnsAsync(results);

    SearchResults<ItemModel> items = await _handler.Handle(query, _cancellationToken);

    Assert.Same(results, items);
  }
}
