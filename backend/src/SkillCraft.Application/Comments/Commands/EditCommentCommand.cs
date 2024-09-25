using FluentValidation;
using MediatR;
using SkillCraft.Application.Comments.Validators;
using SkillCraft.Application.Permissions;
using SkillCraft.Application.Worlds;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Comments;
using SkillCraft.Contracts.Worlds;
using SkillCraft.Domain.Comments;
using Action = SkillCraft.Application.Permissions.Action;

namespace SkillCraft.Application.Comments.Commands;

public record EditCommentCommand(Guid Id, EditCommentPayload Payload) : Activity, IRequest<CommentModel?>;

internal class EditCommentCommandHandler : IRequestHandler<EditCommentCommand, CommentModel?>
{
  private readonly ICommentQuerier _commentQuerier;
  private readonly ICommentRepository _commentRepository;
  private readonly IPermissionService _permissionService;
  private readonly ISender _sender;
  private readonly IWorldQuerier _worldQuerier;

  public EditCommentCommandHandler(
    ICommentQuerier commentQuerier,
    ICommentRepository commentRepository,
    IPermissionService permissionService,
    ISender sender,
    IWorldQuerier worldQuerier)
  {
    _commentQuerier = commentQuerier;
    _commentRepository = commentRepository;
    _permissionService = permissionService;
    _sender = sender;
    _worldQuerier = worldQuerier;
  }

  public async Task<CommentModel?> Handle(EditCommentCommand command, CancellationToken cancellationToken)
  {
    EditCommentPayload payload = command.Payload;
    new EditCommentValidator().ValidateAndThrow(payload);

    CommentId id = new(command.Id);
    Comment? comment = await _commentRepository.LoadAsync(id, cancellationToken);
    if (comment == null)
    {
      return null;
    }

    if (comment.EntityType == EntityType.World)
    {
      WorldModel world = await _worldQuerier.ReadAsync(comment.EntityId.ToGuid(), cancellationToken)
        ?? throw new InvalidOperationException($"The world 'Id={comment.EntityId.ToGuid()}' could not be found.");
      await _permissionService.EnsureCanCommentAsync(command, world, cancellationToken);
    }
    else
    {
      EntityMetadata entity = new(comment.WorldId, comment.Entity);
      await _permissionService.EnsureCanCommentAsync(command, entity, cancellationToken);
    }

    if (comment.OwnerId != command.GetUserId())
    {
      throw new PermissionDeniedException(Action.Update, EntityType.Comment, command.GetUser(), command.GetWorld(), comment.Id.ToGuid());
    }

    comment.Edit(new Text(payload.Text), command.GetUserId());

    await _sender.Send(new SaveCommentCommand(comment), cancellationToken);

    return await _commentQuerier.ReadAsync(comment, cancellationToken);
  }
}
