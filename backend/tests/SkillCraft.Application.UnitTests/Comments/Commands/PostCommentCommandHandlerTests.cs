using FluentValidation.Results;
using Logitar.Portal.Contracts.Users;
using MediatR;
using Moq;
using SkillCraft.Application.Permissions;
using SkillCraft.Application.Worlds;
using SkillCraft.Contracts.Comments;
using SkillCraft.Domain;
using SkillCraft.Domain.Comments;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Comments.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class PostCommentCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<ICommentQuerier> _commentQuerier = new();
  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<ISender> _sender = new();
  private readonly Mock<IWorldQuerier> _worldQuerier = new();
  private readonly Mock<IWorldRepository> _worldRepository = new();

  private readonly PostCommentCommandHandler _handler;

  private readonly User _user;
  private readonly World _world;

  public PostCommentCommandHandlerTests()
  {
    _handler = new(_commentQuerier.Object, _permissionService.Object, _sender.Object, _worldQuerier.Object, _worldRepository.Object);

    _user = new UserMock();
    _world = new(new Slug("ungar"), new UserId(_user.Id));
  }

  [Fact(DisplayName = "It should post a new comment on a world entity.")]
  public async Task It_should_post_a_new_comment_on_a_world_entity()
  {
    PostCommentPayload payload = new(" Hello World! ");
    PostCommentCommand command = new(EntityType.World, _world.Id.ToGuid(), payload);
    command.Contextualize(_user, _world);

    _worldRepository.Setup(x => x.LoadAsync(_world.Id, _cancellationToken)).ReturnsAsync(_world);

    CommentModel model = new();
    _commentQuerier.Setup(x => x.ReadAsync(It.IsAny<Comment>(), _cancellationToken)).ReturnsAsync(model);

    CommentModel? result = await _handler.Handle(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(result, model);

    _permissionService.Verify(x => x.EnsureCanCommentAsync(command, _world, _cancellationToken), Times.Once);

    _sender.Verify(x => x.Send(It.Is<SaveCommentCommand>(y => y.Comment.WorldId == _world.Id
      && y.Comment.EntityType == command.EntityType
      && y.Comment.EntityId.ToGuid() == command.EntityId
      && y.Comment.Text.Value == payload.Text.Trim()), _cancellationToken), Times.Once);

    _worldQuerier.Verify(x => x.FindIdAsync(It.IsAny<EntityKey>(), It.IsAny<CancellationToken>()), Times.Never);
  }

  [Fact(DisplayName = "It should post a new comment on an entity that is not a world.")]
  public async Task It_should_post_a_new_comment_on_an_entity_that_is_not_a_world()
  {
    PostCommentPayload payload = new(" Hello World! ");
    PostCommentCommand command = new(EntityType.Character, Guid.NewGuid(), payload);
    command.Contextualize(_user, _world);

    _worldQuerier.Setup(x => x.FindIdAsync(It.Is<EntityKey>(y => y.Type == command.EntityType && y.Id == command.EntityId), _cancellationToken))
      .ReturnsAsync(_world.Id);

    CommentModel model = new();
    _commentQuerier.Setup(x => x.ReadAsync(It.IsAny<Comment>(), _cancellationToken)).ReturnsAsync(model);

    CommentModel? result = await _handler.Handle(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(result, model);

    _permissionService.Verify(x => x.EnsureCanCommentAsync(
      command,
      It.Is<EntityMetadata>(y => y.WorldId == _world.Id && y.Key.Type == command.EntityType && y.Key.Id == command.EntityId && y.Size == 1),
      _cancellationToken), Times.Once);

    _sender.Verify(x => x.Send(It.Is<SaveCommentCommand>(y => y.Comment.WorldId == _world.Id
      && y.Comment.EntityType == command.EntityType
      && y.Comment.EntityId.ToGuid() == command.EntityId
      && y.Comment.Text.Value == payload.Text.Trim()), _cancellationToken), Times.Once);

    _worldRepository.Verify(x => x.LoadAsync(It.IsAny<WorldId>(), It.IsAny<CancellationToken>()), Times.Never);
  }

  [Fact(DisplayName = "It should return null when the entity could not be found.")]
  public async Task It_should_return_null_when_the_entity_could_not_be_found()
  {
    PostCommentPayload payload = new(" Hello World! ");
    PostCommentCommand command = new(EntityType.Language, Guid.NewGuid(), payload);

    Assert.Null(await _handler.Handle(command, _cancellationToken));
  }

  [Fact(DisplayName = "It should return null when the entity type is not valid.")]
  public async Task It_should_return_null_when_the_entity_type_is_not_valid()
  {
    PostCommentPayload payload = new(" Hello World! ");
    PostCommentCommand command = new(EntityType.Comment, Guid.NewGuid(), payload);

    Assert.Null(await _handler.Handle(command, _cancellationToken));
  }

  [Fact(DisplayName = "It should return null when the world could not be found.")]
  public async Task It_should_return_null_when_the_world_could_not_be_found()
  {
    PostCommentPayload payload = new(" Hello World! ");
    PostCommentCommand command = new(EntityType.World, _world.Id.ToGuid(), payload);

    Assert.Null(await _handler.Handle(command, _cancellationToken));
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    PostCommentPayload payload = new();
    PostCommentCommand command = new(EntityType.Lineage, Guid.Empty, payload);

    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.Handle(command, _cancellationToken));

    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("NotEmptyValidator", error.ErrorCode);
    Assert.Equal("Text", error.PropertyName);
    Assert.Equal(payload.Text, error.AttemptedValue);
  }
}
