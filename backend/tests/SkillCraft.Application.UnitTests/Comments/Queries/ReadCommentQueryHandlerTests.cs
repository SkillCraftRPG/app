using Moq;
using SkillCraft.Application.Permissions;
using SkillCraft.Application.Worlds;
using SkillCraft.Contracts.Comments;
using SkillCraft.Contracts.Worlds;

namespace SkillCraft.Application.Comments.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class ReadCommentQueryHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<ICommentQuerier> _commentQuerier = new();
  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<IWorldQuerier> _worldQuerier = new();

  private readonly ReadCommentQueryHandler _handler;

  private readonly CommentModel _comment = new(new WorldModel(), "Hello World!") { Id = Guid.NewGuid() };

  public ReadCommentQueryHandlerTests()
  {
    _handler = new(_commentQuerier.Object, _permissionService.Object, _worldQuerier.Object);

    _commentQuerier.Setup(x => x.ReadAsync(_comment.Id, _cancellationToken)).ReturnsAsync(_comment);
  }

  [Fact(DisplayName = "It should return null when no comment is found.")]
  public async Task It_should_return_null_when_no_comment_is_found()
  {
    ReadCommentQuery query = new(Guid.Empty);
    Assert.Null(await _handler.Handle(query, _cancellationToken));
  }

  [Fact(DisplayName = "It should return the comment found by ID.")]
  public async Task It_should_return_the_comment_found_by_Id()
  {
    ReadCommentQuery query = new(_comment.Id);
    CommentModel? comment = await _handler.Handle(query, _cancellationToken);
    Assert.Same(_comment, comment);
  }
}
