using Logitar.Portal.Contracts.Search;
using Moq;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Parties;

namespace SkillCraft.Application.Parties.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class SearchPartiesQueryHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IPartyQuerier> _partyQuerier = new();
  private readonly Mock<IPermissionService> _permissionService = new();

  private readonly SearchPartiesQueryHandler _handler;

  private readonly WorldMock _world = new();

  public SearchPartiesQueryHandlerTests()
  {
    _handler = new(_partyQuerier.Object, _permissionService.Object);
  }

  [Fact(DisplayName = "It should return the search results.")]
  public async Task It_should_return_the_search_results()
  {
    SearchPartiesPayload payload = new();
    SearchPartiesQuery query = new(payload);
    query.Contextualize(_world);

    SearchResults<PartyModel> results = new();
    _partyQuerier.Setup(x => x.SearchAsync(_world.Id, payload, _cancellationToken)).ReturnsAsync(results);

    SearchResults<PartyModel> parties = await _handler.Handle(query, _cancellationToken);

    Assert.Same(results, parties);

    _permissionService.Verify(x => x.EnsureCanPreviewAsync(query, EntityType.Party, _cancellationToken), Times.Once);
  }
}
