using Logitar.Portal.Contracts.Search;
using MediatR;
using SkillCraft.Application.Permissions;
using SkillCraft.Application.Worlds;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Comments;
using SkillCraft.Contracts.Worlds;
using SkillCraft.Domain;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Comments.Queries;

public record SearchCommentsQuery : Activity, IRequest<SearchResults<CommentModel>?>
{
  public EntityKey Entity { get; }
  public SearchCommentsPayload Payload { get; }

  public SearchCommentsQuery(EntityType entityType, Guid entityId, SearchCommentsPayload payload)
  {
    Entity = new(entityType, entityId);
    Payload = payload;
  }
}

internal class SearchCommentsQueryHandler : IRequestHandler<SearchCommentsQuery, SearchResults<CommentModel>?>
{
  private readonly ICommentQuerier _commentQuerier;
  private readonly IPermissionService _permissionService;
  private readonly IWorldQuerier _worldQuerier;

  public SearchCommentsQueryHandler(ICommentQuerier commentQuerier, IPermissionService permissionService, IWorldQuerier worldQuerier)
  {
    _commentQuerier = commentQuerier;
    _permissionService = permissionService;
    _worldQuerier = worldQuerier;
  }

  public async Task<SearchResults<CommentModel>?> Handle(SearchCommentsQuery query, CancellationToken cancellationToken)
  {
    EntityKey entity = query.Entity;
    if (!entity.Type.IsGameEntity())
    {
      return null;
    }

    if (entity.Type == EntityType.World)
    {
      WorldModel? world = await _worldQuerier.ReadAsync(entity.Id, cancellationToken);
      if (world == null)
      {
        return null;
      }

      await _permissionService.EnsureCanViewAsync(query, world, cancellationToken);
    }
    else
    {
      WorldId? worldId = await _worldQuerier.FindIdAsync(entity, cancellationToken);
      if (!worldId.HasValue)
      {
        return null;
      }

      EntityMetadata metadata = new(worldId.Value, entity, size: 1);
      await _permissionService.EnsureCanViewAsync(query, metadata, cancellationToken);
    }

    return await _commentQuerier.SearchAsync(entity, query.Payload, cancellationToken);
  }
}
