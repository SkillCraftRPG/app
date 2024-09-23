using FluentValidation;
using MediatR;
using SkillCraft.Application.Comments.Validators;
using SkillCraft.Application.Permissions;
using SkillCraft.Application.Worlds;
using SkillCraft.Contracts.Comments;
using SkillCraft.Domain;
using SkillCraft.Domain.Comments;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Comments.Commands;

public record PostCommentCommand(EntityType EntityType, Guid EntityId, PostCommentPayload Payload) : Activity, IRequest<CommentModel?>;

internal class PostCommentCommandHandler : IRequestHandler<PostCommentCommand, CommentModel?>
{
  private readonly ICommentQuerier _commentQuerier;
  private readonly IPermissionService _permissionService;
  private readonly IWorldQuerier _worldQuerier;
  private readonly IWorldRepository _worldRepository;

  public PostCommentCommandHandler(
    ICommentQuerier commentQuerier,
    IPermissionService permissionService,
    IWorldQuerier worldQuerier,
    IWorldRepository worldRepository)
  {
    _commentQuerier = commentQuerier;
    _permissionService = permissionService;
    _worldQuerier = worldQuerier;
    _worldRepository = worldRepository;
  }

  public async Task<CommentModel?> Handle(PostCommentCommand command, CancellationToken cancellationToken)
  {
    PostCommentPayload payload = command.Payload;
    new PostCommentValidator().ValidateAndThrow(payload);

    EntityKey entity = new(command.EntityType, command.EntityId);
    WorldId? worldId = await _worldQuerier.FindIdAsync(entity, cancellationToken);
    if (!worldId.HasValue)
    {
      return null;
    }

    if (entity.Type == EntityType.World)
    {
      World world = await _worldRepository.LoadAsync(worldId.Value, cancellationToken)
        ?? throw new InvalidOperationException($"The world 'Id={worldId}' could not be found.");
      await _permissionService.EnsureCanCommentAsync(command, world, cancellationToken);
    }
    else
    {
      EntityMetadata entityMetadata = new(worldId.Value, entity, size: 1);
      await _permissionService.EnsureCanCommentAsync(command, entityMetadata, cancellationToken);
    }

    Comment comment = Comment.Post(worldId.Value, entity, new Text(payload.Text), command.GetUserId());

    // TODO(fpion): save; 402 --> no more storage

    return await _commentQuerier.ReadAsync(comment, cancellationToken);
  }
}
