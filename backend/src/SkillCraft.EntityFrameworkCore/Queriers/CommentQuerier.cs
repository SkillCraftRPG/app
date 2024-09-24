using Logitar.EventSourcing;
using Logitar.Portal.Contracts.Actors;
using Microsoft.EntityFrameworkCore;
using SkillCraft.Application.Actors;
using SkillCraft.Application.Comments;
using SkillCraft.Contracts.Comments;
using SkillCraft.Domain.Comments;
using SkillCraft.EntityFrameworkCore.Entities;

namespace SkillCraft.EntityFrameworkCore.Queriers;

internal class CommentQuerier : ICommentQuerier
{
  private readonly IActorService _actorService;
  private readonly DbSet<CommentEntity> _comments;
  private readonly ISqlHelper _sqlHelper;

  public CommentQuerier(IActorService actorService, SkillCraftContext context, ISqlHelper sqlHelper)
  {
    _actorService = actorService;
    _comments = context.Comments;
    _sqlHelper = sqlHelper;
  }

  public async Task<CommentModel> ReadAsync(Comment comment, CancellationToken cancellationToken)
  {
    return await ReadAsync(comment.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The comment entity 'Id={comment.Id.ToGuid()}' could not be found.");
  }
  public async Task<CommentModel?> ReadAsync(CommentId id, CancellationToken cancellationToken)
  {
    return await ReadAsync(id.ToGuid(), cancellationToken);
  }
  public async Task<CommentModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    CommentEntity? comment = await _comments.AsNoTracking()
      .Include(x => x.World)
      .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    return comment == null ? null : await MapAsync(comment, cancellationToken);
  }

  private async Task<CommentModel> MapAsync(CommentEntity comment, CancellationToken cancellationToken)
  {
    return (await MapAsync([comment], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<CommentModel>> MapAsync(IEnumerable<CommentEntity> comments, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = comments.SelectMany(comment => comment.GetActorIds());
    IReadOnlyCollection<Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return comments.Select(mapper.ToComment).ToArray();
  }
}
