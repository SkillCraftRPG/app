using FluentValidation;
using MediatR;
using SkillCraft.Application.Comments.Validators;
using SkillCraft.Application.Permissions;
using SkillCraft.Application.Worlds;
using SkillCraft.Contracts.Comments;
using SkillCraft.Contracts.Worlds;
using SkillCraft.Domain;
using SkillCraft.Domain.Comments;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Comments.Commands;

public record PostCommentCommand : Activity, IRequest<CommentModel?>
{
  public EntityKey Entity { get; }
  public PostCommentPayload Payload { get; }

  public PostCommentCommand(EntityType entityType, Guid entityId, PostCommentPayload payload)
  {
    Entity = new(entityType, entityId);
    Payload = payload;
  }
}

internal class PostCommentCommandHandler : IRequestHandler<PostCommentCommand, CommentModel?>
{
  private readonly ICommentQuerier _commentQuerier;
  private readonly IPermissionService _permissionService;
  private readonly ISender _sender;
  private readonly IWorldQuerier _worldQuerier;

  public PostCommentCommandHandler(
    ICommentQuerier commentQuerier,
    IPermissionService permissionService,
    ISender sender,
    IWorldQuerier worldQuerier)
  {
    _commentQuerier = commentQuerier;
    _permissionService = permissionService;
    _sender = sender;
    _worldQuerier = worldQuerier;
  }

  public async Task<CommentModel?> Handle(PostCommentCommand command, CancellationToken cancellationToken)
  {
    EntityKey entity = command.Entity;
    if (!entity.Type.IsGameEntity())
    {
      return null;
    }

    PostCommentPayload payload = command.Payload;
    new PostCommentValidator().ValidateAndThrow(payload);

    WorldId worldId;
    if (entity.Type == EntityType.World)
    {
      worldId = new(entity.Id);
      WorldModel? world = await _worldQuerier.ReadAsync(worldId, cancellationToken);
      if (world == null)
      {
        return null;
      }

      await _permissionService.EnsureCanCommentAsync(command, world, cancellationToken);
    }
    else
    {
      WorldId? id = await _worldQuerier.FindIdAsync(entity, cancellationToken);
      if (!id.HasValue)
      {
        return null;
      }
      worldId = id.Value;

      EntityMetadata metadata = new(worldId, entity, size: 1);
      await _permissionService.EnsureCanCommentAsync(command, metadata, cancellationToken);
    }

    Comment comment = Comment.Post(worldId, entity, new Text(payload.Text), command.GetUserId());

    await _sender.Send(new SaveCommentCommand(comment), cancellationToken);

    return await _commentQuerier.ReadAsync(comment, cancellationToken);
  }
}
