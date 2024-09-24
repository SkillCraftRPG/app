using FluentValidation.Results;
using Logitar.Portal.Contracts.Users;
using MediatR;
using Moq;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts.Comments;
using SkillCraft.Domain;
using SkillCraft.Domain.Comments;
using SkillCraft.Domain.Worlds;
using Action = SkillCraft.Application.Permissions.Action;

namespace SkillCraft.Application.Comments.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class EditCommentCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<ICommentQuerier> _commentQuerier = new();
  private readonly Mock<ICommentRepository> _commentRepository = new();
  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<ISender> _sender = new();
  private readonly Mock<IWorldRepository> _worldRepository = new();

  private readonly EditCommentCommandHandler _handler;

  private readonly User _user;
  private readonly World _world;

  public EditCommentCommandHandlerTests()
  {
    _handler = new(_commentQuerier.Object, _commentRepository.Object, _permissionService.Object, _sender.Object, _worldRepository.Object);

    _user = new UserMock();
    _world = new(new Slug("ungar"), new UserId(_user.Id));
  }

  [Fact(DisplayName = "It should edit the comment of a world entity.")]
  public async Task It_should_edit_the_comment_of_a_world_entity()
  {
    _worldRepository.Setup(x => x.LoadAsync(_world.Id, _cancellationToken)).ReturnsAsync(_world);

    Comment comment = Comment.Post(_world.Id, new EntityKey(EntityType.World, _world.Id.ToGuid()), new Text("test"), _world.OwnerId);
    _commentRepository.Setup(x => x.LoadAsync(comment.Id, _cancellationToken)).ReturnsAsync(comment);

    EditCommentPayload payload = new("  Hello World!  ");
    EditCommentCommand command = new(comment.Id.ToGuid(), payload);
    command.Contextualize(_user, _world);

    CommentModel model = new();
    _commentQuerier.Setup(x => x.ReadAsync(It.IsAny<Comment>(), _cancellationToken)).ReturnsAsync(model);

    CommentModel? result = await _handler.Handle(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(model, result);

    _permissionService.Verify(x => x.EnsureCanCommentAsync(command, _world, _cancellationToken), Times.Once);

    _sender.Verify(x => x.Send(It.Is<SaveCommentCommand>(y => y.Comment.Text.Value == payload.Text.Trim()
      && y.Comment.UpdatedBy == _world.OwnerId.ActorId), _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should edit the comment of an entity that is not a world.")]
  public async Task It_should_edit_the_comment_of_an_entity_that_is_not_a_world()
  {
    Comment comment = Comment.Post(_world.Id, new EntityKey(EntityType.Talent, Guid.NewGuid()), new Text("test"), _world.OwnerId);
    _commentRepository.Setup(x => x.LoadAsync(comment.Id, _cancellationToken)).ReturnsAsync(comment);

    EditCommentPayload payload = new("  Hello World!  ");
    EditCommentCommand command = new(comment.Id.ToGuid(), payload);
    command.Contextualize(_user, _world);

    CommentModel model = new();
    _commentQuerier.Setup(x => x.ReadAsync(It.IsAny<Comment>(), _cancellationToken)).ReturnsAsync(model);

    CommentModel? result = await _handler.Handle(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(model, result);

    _permissionService.Verify(x => x.EnsureCanCommentAsync(
      command,
      It.Is<EntityMetadata>(y => y.WorldId == _world.Id && y.Type == comment.EntityType && y.Id == comment.EntityId.ToGuid() && y.Size > 0),
      _cancellationToken), Times.Once);

    _sender.Verify(x => x.Send(It.Is<SaveCommentCommand>(y => y.Comment.Text.Value == payload.Text.Trim()
      && y.Comment.UpdatedBy == _world.OwnerId.ActorId), _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should return null when the comment could not be found.")]
  public async Task It_should_return_null_when_the_comment_could_not_be_found()
  {
    EditCommentPayload payload = new("Hello World!");
    EditCommentCommand command = new(Guid.NewGuid(), payload);

    Assert.Null(await _handler.Handle(command, _cancellationToken));
  }

  [Fact(DisplayName = "It should throw InvalidOperationException when the world could not be found.")]
  public async Task It_should_throw_InvalidOperationException_when_the_world_could_not_be_found()
  {
    Comment comment = Comment.Post(_world.Id, new EntityKey(EntityType.World, _world.Id.ToGuid()), new Text("test"), _world.OwnerId);
    _commentRepository.Setup(x => x.LoadAsync(comment.Id, _cancellationToken)).ReturnsAsync(comment);

    EditCommentPayload payload = new("Hello World!");
    EditCommentCommand command = new(comment.Id.ToGuid(), payload);

    var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () => await _handler.Handle(command, _cancellationToken));
    Assert.StartsWith($"The world 'Id={_world.Id}' could not be found.", exception.Message);
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when the user does not own the comment.")]
  public async Task It_should_throw_PermissionDeniedException_when_the_user_does_not_own_the_comment()
  {
    _worldRepository.Setup(x => x.LoadAsync(_world.Id, _cancellationToken)).ReturnsAsync(_world);

    Comment comment = Comment.Post(_world.Id, new EntityKey(EntityType.World, _world.Id.ToGuid()), new Text("test"), UserId.NewId());
    _commentRepository.Setup(x => x.LoadAsync(comment.Id, _cancellationToken)).ReturnsAsync(comment);

    EditCommentPayload payload = new("Hello World!");
    EditCommentCommand command = new(comment.Id.ToGuid(), payload);
    command.Contextualize(_user, _world);

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _handler.Handle(command, _cancellationToken));
    Assert.Equal(Action.Update, exception.Action);
    Assert.Equal(EntityType.Comment, exception.EntityType);
    Assert.Equal(command.GetUser().Id, exception.UserId);
    Assert.Equal(command.GetWorld().Id, exception.WorldId);
    Assert.Equal(comment.Id.ToGuid(), exception.EntityId);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    EditCommentPayload payload = new();
    EditCommentCommand command = new(Guid.NewGuid(), payload);

    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.Handle(command, _cancellationToken));

    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("NotEmptyValidator", error.ErrorCode);
    Assert.Equal("Text", error.PropertyName);
    Assert.Equal(payload.Text, error.AttemptedValue);
  }
}
