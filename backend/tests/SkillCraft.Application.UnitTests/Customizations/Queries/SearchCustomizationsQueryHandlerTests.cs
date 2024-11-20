using Logitar.Portal.Contracts.Search;
using Moq;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Customizations;

namespace SkillCraft.Application.Customizations.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class SearchCustomizationsQueryHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<ICustomizationQuerier> _customizationQuerier = new();
  private readonly Mock<IPermissionService> _permissionService = new();

  private readonly SearchCustomizationsQueryHandler _handler;

  public SearchCustomizationsQueryHandlerTests()
  {
    _handler = new(_customizationQuerier.Object, _permissionService.Object);
  }

  [Fact(DisplayName = "It should return the search results.")]
  public async Task It_should_return_the_search_results()
  {
    SearchCustomizationsPayload payload = new();
    SearchCustomizationsQuery query = new(payload);
    query.Contextualize(new WorldMock());

    SearchResults<CustomizationModel> results = new();
    _customizationQuerier.Setup(x => x.SearchAsync(query.GetWorldId(), payload, _cancellationToken)).ReturnsAsync(results);

    SearchResults<CustomizationModel> customizations = await _handler.Handle(query, _cancellationToken);

    Assert.Same(results, customizations);

    _permissionService.Verify(x => x.EnsureCanPreviewAsync(query, EntityType.Customization, _cancellationToken), Times.Once);
  }
}
