using Logitar.Portal.Contracts.Search;
using Moq;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Characters;

namespace SkillCraft.Application.Characters.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class SearchCharactersQueryHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<ICharacterQuerier> _characterQuerier = new();
  private readonly Mock<IPermissionService> _permissionService = new();

  private readonly SearchCharactersQueryHandler _handler;

  public SearchCharactersQueryHandlerTests()
  {
    _handler = new(_characterQuerier.Object, _permissionService.Object);
  }

  [Fact(DisplayName = "It should return the search results.")]
  public async Task It_should_return_the_search_results()
  {
    SearchCharactersPayload payload = new();
    SearchCharactersQuery query = new(payload);
    query.Contextualize(new WorldMock());

    SearchResults<CharacterModel> results = new();
    _characterQuerier.Setup(x => x.SearchAsync(query.GetWorldId(), payload, _cancellationToken)).ReturnsAsync(results);

    SearchResults<CharacterModel> characters = await _handler.Handle(query, _cancellationToken);

    Assert.Same(results, characters);

    _permissionService.Verify(x => x.EnsureCanPreviewAsync(query, EntityType.Character, _cancellationToken), Times.Once);
  }
}
