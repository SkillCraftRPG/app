using MediatR;
using SkillCraft.Application.Storages;
using SkillCraft.Domain.Comments;

namespace SkillCraft.Application.Comments.Commands;

internal record SaveCommentCommand(Comment Comment) : IRequest;

internal class SaveCommentCommandHandler : IRequestHandler<SaveCommentCommand>
{
  private readonly ICommentRepository _commentRepository;
  private readonly IStorageService _storageService;

  public SaveCommentCommandHandler(ICommentRepository commentRepository, IStorageService storageService)
  {
    _commentRepository = commentRepository;
    _storageService = storageService;
  }

  public async Task Handle(SaveCommentCommand command, CancellationToken cancellationToken)
  {
    Comment comment = command.Comment;

    EntityMetadata entity = comment.GetMetadata();
    await _storageService.EnsureAvailableAsync(entity, cancellationToken);

    await _commentRepository.SaveAsync(comment, cancellationToken);

    await _storageService.UpdateAsync(entity, cancellationToken);
  }
}
