using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillCraft.Application;
using SkillCraft.Application.Comments.Commands;
using SkillCraft.Application.Comments.Queries;
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
  private readonly IRequestPipeline _pipeline;

  public CommentController(IRequestPipeline pipeline)
  {
    _pipeline = pipeline;
  }

  [HttpPut("/comments/{id}")]
  public async Task<ActionResult<CommentModel>> EditAsync(Guid id, [FromBody] EditCommentPayload payload, CancellationToken cancellationToken)
  {
    CommentModel? comment = await _pipeline.ExecuteAsync(new EditCommentCommand(id, payload), cancellationToken);
    return comment == null ? NotFound() : Ok(comment);
  }

  [HttpPost($"/{{entityTypePlural}}/{{entityId}}/{Routes.Comment}")]
  public async Task<ActionResult<CommentModel>> PostAsync(string entityTypePlural, Guid entityId, [FromBody] PostCommentPayload payload, CancellationToken cancellationToken)
  {
    EntityType? entityType = Routes.GetEntityType(entityTypePlural);
    if (entityType == null || entityType == EntityType.Comment)
    {
      return NotFound();
    }

    CommentModel? comment = await _pipeline.ExecuteAsync(new PostCommentCommand(entityType.Value, entityId, payload), cancellationToken);
    if (comment == null)
    {
      return NotFound();
    }

    Uri location = HttpContext.BuildLocation($"{Routes.Comment}/{{id}}", [new KeyValuePair<string, string>("id", comment.Id.ToString())]);

    return Created(location, comment);
  }

  [HttpGet("/comments/{id}")]
  public async Task<ActionResult<CommentModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    CommentModel? comment = await _pipeline.ExecuteAsync(new ReadCommentQuery(id), cancellationToken);
    return comment == null ? NotFound() : Ok(comment);
  }
}
