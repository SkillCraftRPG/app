using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillCraft.Application;
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
  private readonly IRequestPipeline _pipeline;

  public CommentController(IRequestPipeline pipeline)
  {
    _pipeline = pipeline;
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
}
