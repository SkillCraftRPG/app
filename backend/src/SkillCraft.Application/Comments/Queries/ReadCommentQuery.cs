using MediatR;
using SkillCraft.Contracts.Comments;

namespace SkillCraft.Application.Comments.Queries;

public record ReadCommentQuery(Guid Id) : IRequest<CommentModel?>;

internal class ReadCommentQueryHandler : IRequestHandler<ReadCommentQuery, CommentModel?>
{
  private readonly ICommentQuerier _commentQuerier;

  public ReadCommentQueryHandler(ICommentQuerier commentQuerier)
  {
    _commentQuerier = commentQuerier;
  }

  public async Task<CommentModel?> Handle(ReadCommentQuery query, CancellationToken cancellationToken)
  {
    CommentModel? comment = await _commentQuerier.ReadAsync(query.Id, cancellationToken);
    // TODO(fpion): check permissions

    return comment;
  }
}
