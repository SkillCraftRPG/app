using Logitar.Portal.Contracts.Search;
using Moq;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Talents;

namespace SkillCraft.Application.Talents.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class SearchTalentsQueryHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<ITalentQuerier> _talentQuerier = new();

  private readonly SearchTalentsQueryHandler _handler;

  public SearchTalentsQueryHandlerTests()
  {
    _handler = new(_permissionService.Object, _talentQuerier.Object);
  }

  [Fact(DisplayName = "It should return the search results.")]
  public async Task It_should_return_the_search_results()
  {
    SearchTalentsPayload payload = new();
    SearchTalentsQuery query = new(payload);
    query.Contextualize(new WorldMock());

    SearchResults<TalentModel> results = new();
    _talentQuerier.Setup(x => x.SearchAsync(query.GetWorldId(), payload, _cancellationToken)).ReturnsAsync(results);

    SearchResults<TalentModel> talents = await _handler.Handle(query, _cancellationToken);

    Assert.Same(results, talents);

    _permissionService.Verify(x => x.EnsureCanPreviewAsync(query, EntityType.Talent, _cancellationToken), Times.Once);
  }
}
