using MediatR;
using SkillCraft.Domain.Comments;

namespace SkillCraft.Application.Comments.Commands;

internal record SaveCommentCommand(Comment Comment) : IRequest;

internal class SaveCommentCommandHandler : IRequestHandler<SaveCommentCommand>
{
  private readonly ICommentRepository _commentRepository;

  public SaveCommentCommandHandler(ICommentRepository commentRepository)
  {
    _commentRepository = commentRepository;
  }

  public async Task Handle(SaveCommentCommand command, CancellationToken cancellationToken)
  {
    Comment comment = command.Comment;

    // TODO(fpion): Storage.EnsureAvailableAsync

    await _commentRepository.SaveAsync(comment, cancellationToken);

    // TODO(fpion): Storage.UpdateAsync
  }
}
