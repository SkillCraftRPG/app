using Logitar.Portal.Contracts.Search;
using Moq;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Educations;

namespace SkillCraft.Application.Educations.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class SearchEducationsQueryHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IEducationQuerier> _educationQuerier = new();
  private readonly Mock<IPermissionService> _permissionService = new();

  private readonly SearchEducationsQueryHandler _handler;

  public SearchEducationsQueryHandlerTests()
  {
    _handler = new(_educationQuerier.Object, _permissionService.Object);
  }

  [Fact(DisplayName = "It should return the search results.")]
  public async Task It_should_return_the_search_results()
  {
    SearchEducationsPayload payload = new();
    SearchEducationsQuery query = new(payload);
    query.Contextualize(new WorldMock());

    SearchResults<EducationModel> results = new();
    _educationQuerier.Setup(x => x.SearchAsync(query.GetWorldId(), payload, _cancellationToken)).ReturnsAsync(results);

    SearchResults<EducationModel> educations = await _handler.Handle(query, _cancellationToken);

    Assert.Same(results, educations);

    _permissionService.Verify(x => x.EnsureCanPreviewAsync(query, EntityType.Education, _cancellationToken), Times.Once);
  }
}
