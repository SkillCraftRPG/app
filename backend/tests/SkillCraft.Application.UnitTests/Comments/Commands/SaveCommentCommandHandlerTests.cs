using Moq;
using SkillCraft.Application.Storages;
using SkillCraft.Contracts;
using SkillCraft.Domain;
using SkillCraft.Domain.Comments;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Comments.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class SaveCommentCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;
  private readonly Comment _comment = Comment.Post(WorldId.NewId(), new EntityKey(EntityType.Education, Guid.NewGuid()), new Text("Hello World!"), UserId.NewId());

  private readonly Mock<ICommentRepository> _commentRepository = new();
  private readonly Mock<IStorageService> _storageService = new();

  private readonly SaveCommentCommandHandler _handler;

  public SaveCommentCommandHandlerTests()
  {
    _handler = new(_commentRepository.Object, _storageService.Object);
  }

  [Fact(DisplayName = "It should save the comment.")]
  public async Task It_should_save_the_comment()
  {
    SaveCommentCommand command = new(_comment);

    await _handler.Handle(command, _cancellationToken);

    _commentRepository.Verify(x => x.SaveAsync(_comment, _cancellationToken), Times.Once);

    EntityMetadata entity = _comment.GetMetadata();
    _storageService.Verify(x => x.EnsureAvailableAsync(entity, _cancellationToken), Times.Once);
    _storageService.Verify(x => x.UpdateAsync(entity, _cancellationToken), Times.Once);
  }
}
