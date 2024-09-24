using Logitar.Portal.Contracts.Search;
using Moq;
using SkillCraft.Application.Permissions;
using SkillCraft.Application.Worlds;
using SkillCraft.Contracts.Comments;
using SkillCraft.Domain;

namespace SkillCraft.Application.Comments.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class SearchCommentsQueryHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<ICommentQuerier> _commentQuerier = new();
  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<IWorldQuerier> _worldQuerier = new();

  private readonly SearchCommentsQueryHandler _handler;

  public SearchCommentsQueryHandlerTests()
  {
    _handler = new(_commentQuerier.Object, _permissionService.Object, _worldQuerier.Object);
  }

  [Fact(DisplayName = "It should return null when the entity is not a game entity.")]
  public async Task It_should_return_null_when_the_entity_is_not_a_game_entity()
  {
    SearchCommentsPayload payload = new();
    SearchCommentsQuery query = new(EntityType.Comment, Guid.NewGuid(), payload);

    Assert.Null(await _handler.Handle(query, _cancellationToken));
  }

  [Fact(DisplayName = "It should return the search results.")]
  public async Task It_should_return_the_search_results()
  {
    SearchCommentsPayload payload = new();
    SearchCommentsQuery query = new(EntityType.Education, Guid.NewGuid(), payload);
    query.Contextualize(new WorldMock());

    SearchResults<CommentModel> results = new();
    _commentQuerier.Setup(x => x.SearchAsync(It.Is<EntityKey>(y => y.Type == query.Entity.Type && y.Id == query.Entity.Id), payload, _cancellationToken))
      .ReturnsAsync(results);

    SearchResults<CommentModel>? comments = await _handler.Handle(query, _cancellationToken);

    Assert.NotNull(comments);
    Assert.Same(results, comments);
  }
}
