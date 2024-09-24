using Logitar.Portal.Contracts.Search;
using MediatR;
using SkillCraft.Contracts.Comments;
using SkillCraft.Domain;

namespace SkillCraft.Application.Comments.Queries;

public record SearchCommentsQuery(EntityType EntityType, Guid EntityId, SearchCommentsPayload Payload) : Activity, IRequest<SearchResults<CommentModel>?>;

internal class SearchCommentsQueryHandler : IRequestHandler<SearchCommentsQuery, SearchResults<CommentModel>?>
{
  private readonly ICommentQuerier _commentQuerier;

  public SearchCommentsQueryHandler(ICommentQuerier commentQuerier)
  {
    _commentQuerier = commentQuerier;
  }

  public async Task<SearchResults<CommentModel>?> Handle(SearchCommentsQuery query, CancellationToken cancellationToken)
  {
    if (!query.EntityType.IsGameEntity())
    {
      return null;
    }

    // TODO(fpion): check permissions

    EntityKey entity = new(query.EntityType, query.EntityId);
    return await _commentQuerier.SearchAsync(entity, query.Payload, cancellationToken);
  }
}
