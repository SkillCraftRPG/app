using Logitar.Portal.Contracts.Actors;
using Moq;
using SkillCraft.Application.Permissions;
using SkillCraft.Application.Worlds;
using SkillCraft.Contracts.Comments;
using SkillCraft.Contracts.Worlds;
using SkillCraft.Domain;

namespace SkillCraft.Application.Comments.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class ReadCommentQueryHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<ICommentQuerier> _commentQuerier = new();
  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<IWorldQuerier> _worldQuerier = new();

  private readonly ReadCommentQueryHandler _handler;

  public ReadCommentQueryHandlerTests()
  {
    _handler = new(_commentQuerier.Object, _permissionService.Object, _worldQuerier.Object);
  }

  [Fact(DisplayName = "It should return null when no comment is found.")]
  public async Task It_should_return_null_when_no_comment_is_found()
  {
    ReadCommentQuery query = new(Guid.Empty);
    Assert.Null(await _handler.Handle(query, _cancellationToken));
  }

  [Fact(DisplayName = "It should return the comment found by ID (Entity).")]
  public async Task It_should_return_the_comment_found_by_Id_Entity()
  {
    WorldModel world = new(new Actor(new UserMock()), "ungar")
    {
      Id = Guid.NewGuid()
    };
    CommentModel comment = new(world, "Hello World!")
    {
      Id = Guid.NewGuid(),
      EntityType = EntityType.Aspect.ToString(),
      EntityId = Guid.NewGuid()
    };
    _commentQuerier.Setup(x => x.ReadAsync(comment.Id, _cancellationToken)).ReturnsAsync(comment);

    ReadCommentQuery query = new(comment.Id);
    CommentModel? result = await _handler.Handle(query, _cancellationToken);
    Assert.Same(comment, result);

    _permissionService.Verify(x => x.EnsureCanViewAsync(
      query,
      It.Is<EntityMetadata>(y => y.WorldId.ToGuid() == world.Id && y.Key.Type.ToString() == comment.EntityType && y.Key.Id == comment.EntityId && y.Size > 0),
      _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should return the comment found by ID (World).")]
  public async Task It_should_return_the_comment_found_by_Id_World()
  {
    WorldModel world = new(new Actor(new UserMock()), "ungar")
    {
      Id = Guid.NewGuid()
    };
    CommentModel comment = new(world, "Hello World!")
    {
      Id = Guid.NewGuid(),
      EntityType = EntityType.World.ToString(),
      EntityId = world.Id
    };
    _commentQuerier.Setup(x => x.ReadAsync(comment.Id, _cancellationToken)).ReturnsAsync(comment);

    ReadCommentQuery query = new(comment.Id);
    CommentModel? result = await _handler.Handle(query, _cancellationToken);
    Assert.Same(comment, result);

    _permissionService.Verify(x => x.EnsureCanViewAsync(query, world, _cancellationToken), Times.Once);
  }
}
