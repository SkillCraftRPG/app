using MediatR;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Comments;
using SkillCraft.Domain;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Comments.Queries;

public record ReadCommentQuery(Guid Id) : Activity, IRequest<CommentModel?>;

internal class ReadCommentQueryHandler : IRequestHandler<ReadCommentQuery, CommentModel?>
{
  private readonly ICommentQuerier _commentQuerier;
  private readonly IPermissionService _permissionService;

  public ReadCommentQueryHandler(ICommentQuerier commentQuerier, IPermissionService permissionService)
  {
    _commentQuerier = commentQuerier;
    _permissionService = permissionService;
  }

  public async Task<CommentModel?> Handle(ReadCommentQuery query, CancellationToken cancellationToken)
  {
    CommentModel? comment = await _commentQuerier.ReadAsync(query.Id, cancellationToken);
    if (comment != null)
    {
      WorldId worldId = new(comment.World.Id);
      if (comment.EntityType == EntityType.World)
      {
        await _permissionService.EnsureCanViewAsync(query, comment.World, cancellationToken);
      }
      else
      {
        EntityMetadata entity = new(worldId, new EntityKey(comment.EntityType, comment.EntityId), size: 1);
        await _permissionService.EnsureCanViewAsync(query, entity, cancellationToken);
      }
    }

    return comment;
  }
}
