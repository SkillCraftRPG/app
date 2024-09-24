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
  private readonly ISender _sender;
  private readonly IWorldQuerier _worldQuerier;
  private readonly IWorldRepository _worldRepository;

  public PostCommentCommandHandler(
    ICommentQuerier commentQuerier,
    IPermissionService permissionService,
    ISender sender,
    IWorldQuerier worldQuerier,
    IWorldRepository worldRepository)
  {
    _commentQuerier = commentQuerier;
    _permissionService = permissionService;
    _sender = sender;
    _worldQuerier = worldQuerier;
    _worldRepository = worldRepository;
  }

  public async Task<CommentModel?> Handle(PostCommentCommand command, CancellationToken cancellationToken)
  {
    if (!command.EntityType.IsGameEntity())
    {
      return null;
    }

    PostCommentPayload payload = command.Payload;
    new PostCommentValidator().ValidateAndThrow(payload);

    EntityKey entity = new(command.EntityType, command.EntityId);
    WorldId worldId;
    if (command.EntityType == EntityType.World)
    {
      worldId = new(command.EntityId);
      World? world = await _worldRepository.LoadAsync(worldId, cancellationToken); // TODO(fpion): use querier?
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
