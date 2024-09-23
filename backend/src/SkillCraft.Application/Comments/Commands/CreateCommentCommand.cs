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

public record CreateCommentCommand(EntityKey Key, CreateCommentPayload Payload) : Activity, IRequest<CommentModel?>;

internal class CreateCommentCommandHandler : IRequestHandler<CreateCommentCommand, CommentModel?>
{
  private readonly ICommentQuerier _commentQuerier;
  private readonly IPermissionService _permissionService;
  private readonly ISender _sender;
  private readonly IWorldQuerier _worldQuerier;

  public CreateCommentCommandHandler(
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

  public async Task<CommentModel?> Handle(CreateCommentCommand command, CancellationToken cancellationToken)
  {
    CreateCommentPayload payload = command.Payload;
    new CreateCommentValidator().ValidateAndThrow(payload);

    WorldId? worldId = await _worldQuerier.FindIdAsync(command.Key, cancellationToken);
    if (worldId == null)
    {
      return null;
    }

    EntityMetadata entity = new(worldId.Value, command.Key, size: 1);
    await _permissionService.EnsureCanCommentAsync(command, entity, cancellationToken);

    Comment comment = new(new Text(payload.Text), command.GetUserId());

    await _sender.Send(new SaveCommentCommand(comment), cancellationToken);

    return await _commentQuerier.ReadAsync(comment, cancellationToken);
  }
}
