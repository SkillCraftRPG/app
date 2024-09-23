using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillCraft.Application.Comments.Commands;
using SkillCraft.Constants;
using SkillCraft.Contracts.Comments;
using SkillCraft.Domain;
using SkillCraft.Extensions;
using SkillCraft.Filters;

namespace SkillCraft.Controllers;

[ApiController]
[Authorize]
[RequireWorld]
public class CommentController : ControllerBase
{
  private readonly ISender _sender;

  public CommentController(ISender sender)
  {
    _sender = sender;
  }

  [HttpPost("/{entityTypePlural}/{entityId}/comments")]
  public async Task<ActionResult<CommentModel>> CreateAsync(string entityTypePlural, Guid entityId, [FromBody] CreateCommentPayload payload, CancellationToken cancellationToken)
  {
    EntityType? entityType = Routes.GetEntityType(entityTypePlural);
    if (entityType == null)
    {
      return NotFound();
    }

    CommentModel? comment = await _sender.Send(new CreateCommentCommand(new EntityKey(entityType.Value, entityId), payload), cancellationToken);
    if (comment == null)
    {
      return NotFound();
    }

    Uri location = HttpContext.BuildLocation("comments/{id}", [new KeyValuePair<string, string>("id", comment.Id.ToString())]);

    return Created(location, comment);
  }

  [HttpGet("/comments/{id}")]
  public async Task<ActionResult<CommentModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    await Task.Delay(1000, cancellationToken);
    return StatusCode(StatusCodes.Status501NotImplemented); // TODO(fpion): implement
  }
}
