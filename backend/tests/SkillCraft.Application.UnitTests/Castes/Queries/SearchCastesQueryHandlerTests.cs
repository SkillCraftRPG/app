using Logitar.Portal.Contracts.Search;
using Moq;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Castes;

namespace SkillCraft.Application.Castes.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class SearchCastesQueryHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<ICasteQuerier> _casteQuerier = new();
  private readonly Mock<IPermissionService> _permissionService = new();

  private readonly SearchCastesQueryHandler _handler;

  public SearchCastesQueryHandlerTests()
  {
    _handler = new(_casteQuerier.Object, _permissionService.Object);
  }

  [Fact(DisplayName = "It should return the search results.")]
  public async Task It_should_return_the_search_results()
  {
    SearchCastesPayload payload = new();
    SearchCastesQuery query = new(payload);
    query.Contextualize(new WorldMock());

    SearchResults<CasteModel> results = new();
    _casteQuerier.Setup(x => x.SearchAsync(query.GetWorldId(), payload, _cancellationToken)).ReturnsAsync(results);

    SearchResults<CasteModel> castes = await _handler.Handle(query, _cancellationToken);

    Assert.Same(results, castes);

    _permissionService.Verify(x => x.EnsureCanPreviewAsync(query, EntityType.Caste, _cancellationToken), Times.Once);
  }
}
