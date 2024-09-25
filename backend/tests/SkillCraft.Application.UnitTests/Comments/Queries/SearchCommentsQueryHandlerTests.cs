using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Search;
using Moq;
using SkillCraft.Application.Permissions;
using SkillCraft.Application.Worlds;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Comments;
using SkillCraft.Contracts.Worlds;
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

  private readonly UserMock _user = new();
  private readonly WorldMock _world;

  public SearchCommentsQueryHandlerTests()
  {
    _handler = new(_commentQuerier.Object, _permissionService.Object, _worldQuerier.Object);

    _world = new(_user);
  }

  [Fact(DisplayName = "It should return null when the entity is not a game entity.")]
  public async Task It_should_return_null_when_the_entity_is_not_a_game_entity()
  {
    SearchCommentsPayload payload = new();
    SearchCommentsQuery query = new(EntityType.Comment, Guid.NewGuid(), payload);

    Assert.Null(await _handler.Handle(query, _cancellationToken));
  }

  [Fact(DisplayName = "It should return null when the entity could not be found.")]
  public async Task It_should_return_null_when_the_entity_could_not_be_found()
  {
    SearchCommentsPayload payload = new();
    SearchCommentsQuery query = new(EntityType.Education, Guid.NewGuid(), payload);
    query.Contextualize(_user, _world);

    Assert.Null(await _handler.Handle(query, _cancellationToken));
  }

  [Fact(DisplayName = "It should return null when the world entity could not be found.")]
  public async Task It_should_return_null_when_the_world_entity_could_not_be_found()
  {
    SearchCommentsPayload payload = new();
    SearchCommentsQuery query = new(EntityType.World, _world.Id.ToGuid(), payload);
    query.Contextualize(_user, _world);

    Assert.Null(await _handler.Handle(query, _cancellationToken));
  }

  [Fact(DisplayName = "It should return the entity comments.")]
  public async Task It_should_return_the_entity_comments()
  {
    SearchCommentsPayload payload = new();
    SearchCommentsQuery query = new(EntityType.Caste, Guid.NewGuid(), payload);
    query.Contextualize(_user, _world);

    _worldQuerier.Setup(x => x.FindIdAsync(query.Entity, _cancellationToken)).ReturnsAsync(_world.Id);

    SearchResults<CommentModel> results = new();
    _commentQuerier.Setup(x => x.SearchAsync(It.Is<EntityKey>(y => y.Type == query.Entity.Type && y.Id == query.Entity.Id), payload, _cancellationToken))
      .ReturnsAsync(results);

    SearchResults<CommentModel>? comments = await _handler.Handle(query, _cancellationToken);

    Assert.NotNull(comments);
    Assert.Same(results, comments);

    _permissionService.Verify(x => x.EnsureCanViewAsync(
      query,
      It.Is<EntityMetadata>(y => y.WorldId == _world.Id && y.Key == query.Entity && y.Size == 1),
      _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should return the world entity comments.")]
  public async Task It_should_return_the_world_entity_comments()
  {
    SearchCommentsPayload payload = new();
    SearchCommentsQuery query = new(EntityType.World, _world.Id.ToGuid(), payload);
    query.Contextualize(_user, _world);

    WorldModel world = new(new Actor(_user), _world.Slug.Value)
    {
      Id = _world.Id.ToGuid()
    };
    _worldQuerier.Setup(x => x.ReadAsync(world.Id, _cancellationToken)).ReturnsAsync(world);

    SearchResults<CommentModel> results = new();
    _commentQuerier.Setup(x => x.SearchAsync(It.Is<EntityKey>(y => y.Type == query.Entity.Type && y.Id == query.Entity.Id), payload, _cancellationToken))
      .ReturnsAsync(results);

    SearchResults<CommentModel>? comments = await _handler.Handle(query, _cancellationToken);

    Assert.NotNull(comments);
    Assert.Same(results, comments);

    _permissionService.Verify(x => x.EnsureCanViewAsync(query, world, _cancellationToken), Times.Once);
  }
}
