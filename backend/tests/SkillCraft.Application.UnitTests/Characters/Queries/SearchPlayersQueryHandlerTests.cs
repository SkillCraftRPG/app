using Bogus;
using Logitar.Portal.Contracts.Search;
using Moq;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;

namespace SkillCraft.Application.Characters.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class SearchPlayersQueryHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;
  private readonly Faker _faker = new();

  private readonly Mock<ICharacterQuerier> _characterQuerier = new();
  private readonly Mock<IPermissionService> _permissionService = new();

  private readonly SearchPlayersQueryHandler _handler;

  public SearchPlayersQueryHandlerTests()
  {
    _handler = new(_characterQuerier.Object, _permissionService.Object);
  }

  [Fact(DisplayName = "It should list the players.")]
  public async Task It_should_list_the_players()
  {
    SearchPlayersQuery query = new();
    query.Contextualize(new WorldMock());

    string[] results = [_faker.Person.FullName, _faker.Name.FullName()];
    _characterQuerier.Setup(x => x.ListPlayersAsync(query.GetWorldId(), _cancellationToken)).ReturnsAsync(results);

    SearchResults<string> players = await _handler.Handle(query, _cancellationToken);

    Assert.Equal(results, players.Items);
    Assert.Equal(results.Length, players.Total);

    _permissionService.Verify(x => x.EnsureCanPreviewAsync(query, EntityType.Character, _cancellationToken), Times.Once);
  }
}
