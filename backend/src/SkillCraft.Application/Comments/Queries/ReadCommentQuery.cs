using MediatR;
using SkillCraft.Application.Permissions;
using SkillCraft.Application.Worlds;
using SkillCraft.Contracts.Comments;

namespace SkillCraft.Application.Comments.Queries;

public record ReadCommentQuery(Guid Id) : IRequest<CommentModel?>;

internal class ReadCommentQueryHandler : IRequestHandler<ReadCommentQuery, CommentModel?>
{
  private readonly ICommentQuerier _commentQuerier;
  private readonly IPermissionService _permissionService;
  private readonly IWorldQuerier _worldQuerier;

  public ReadCommentQueryHandler(ICommentQuerier commentQuerier, IPermissionService permissionService, IWorldQuerier worldQuerier)
  {
    _commentQuerier = commentQuerier;
    _permissionService = permissionService;
    _worldQuerier = worldQuerier;
  }

  public async Task<CommentModel?> Handle(ReadCommentQuery query, CancellationToken cancellationToken)
  {
    CommentModel? comment = await _commentQuerier.ReadAsync(query.Id, cancellationToken);
    if (comment != null)
    {
      // TODO(fpion): check permissions; ensure can view entity
    }

    return comment;
  }
}
