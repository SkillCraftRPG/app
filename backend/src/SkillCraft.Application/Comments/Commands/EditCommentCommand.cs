using FluentValidation;
using MediatR;
using SkillCraft.Application.Comments.Validators;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts.Comments;
using SkillCraft.Domain;
using SkillCraft.Domain.Comments;
using SkillCraft.Domain.Worlds;
using Action = SkillCraft.Application.Permissions.Action;

namespace SkillCraft.Application.Comments.Commands;

public record EditCommentCommand(Guid Id, EditCommentPayload Payload) : Activity, IRequest<CommentModel?>;

internal class EditCommentCommandHandler : IRequestHandler<EditCommentCommand, CommentModel?>
{
  private readonly ICommentQuerier _commentQuerier;
  private readonly ICommentRepository _commentRepository;
  private readonly IPermissionService _permissionService;
  private readonly ISender _sender;
  private readonly IWorldRepository _worldRepository;

  public EditCommentCommandHandler(
    ICommentQuerier commentQuerier,
    ICommentRepository commentRepository,
    IPermissionService permissionService,
    ISender sender,
    IWorldRepository worldRepository)
  {
    _commentQuerier = commentQuerier;
    _commentRepository = commentRepository;
    _permissionService = permissionService;
    _sender = sender;
    _worldRepository = worldRepository;
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
      WorldId worldId = new(comment.EntityId);
      World world = await _worldRepository.LoadAsync(worldId, cancellationToken) // TODO(fpion): use querier?
        ?? throw new InvalidOperationException($"The world 'Id={worldId}' could not be found.");
      await _permissionService.EnsureCanCommentAsync(command, world, cancellationToken);
    }
    else
    {
      EntityMetadata entity = new(comment.WorldId, comment.Entity, size: 1);
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
