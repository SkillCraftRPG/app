using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Search;
using Microsoft.EntityFrameworkCore;
using SkillCraft.Application.Actors;
using SkillCraft.Application.Comments;
using SkillCraft.Contracts.Comments;
using SkillCraft.Domain;
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

  public async Task<SearchResults<CommentModel>> SearchAsync(EntityKey entity, SearchCommentsPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _sqlHelper.QueryFrom(SkillCraftDb.Comments.Table).SelectAll(SkillCraftDb.Comments.Table)
      .Where(SkillCraftDb.Comments.EntityType, Operators.IsEqualTo(entity.Type.ToString()))
      .Where(SkillCraftDb.Comments.EntityId, Operators.IsEqualTo(entity.Id));

    IQueryable<CommentEntity> query = _comments.FromQuery(builder).AsNoTracking()
      .Include(x => x.World);

    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<CommentEntity>? ordered = null;
    foreach (CommentSortOption sort in payload.Sort)
    {
      switch (sort.Field)
      {
        case CommentSort.CreatedOn:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.CreatedOn) : query.OrderBy(x => x.CreatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.CreatedOn) : ordered.ThenBy(x => x.CreatedOn));
          break;
        case CommentSort.UpdatedOn:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn));
          break;
      }
    }
    query = ordered ?? query;
    query = query.ApplyPaging(payload.Skip, payload.Limit);

    CommentEntity[] comments = await query.ToArrayAsync(cancellationToken);
    IEnumerable<CommentModel> items = await MapAsync(comments, cancellationToken);

    return new SearchResults<CommentModel>(items, total);
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
