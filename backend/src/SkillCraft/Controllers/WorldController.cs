using Logitar.Portal.Contracts.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillCraft.Contracts.Worlds;

namespace SkillCraft.Controllers;

[ApiController]
[Authorize]
[Route("worlds")]
public class WorldController : ControllerBase
{
  [HttpPost]
  public async Task<ActionResult<World>> CreateAsync([FromBody] CreateWorldPayload payload, CancellationToken cancellationToken)
  {
    await Task.Delay(1, cancellationToken);
    return StatusCode(StatusCodes.Status501NotImplemented); // TODO(fpion): implement
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<World>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    await Task.Delay(1, cancellationToken);
    return StatusCode(StatusCodes.Status501NotImplemented); // TODO(fpion): implement
  }

  [HttpGet("unique-slug:{uniqueSlug}")]
  public async Task<ActionResult<World>> ReadAsync(string uniqueSlug, CancellationToken cancellationToken)
  {
    await Task.Delay(1, cancellationToken);
    return StatusCode(StatusCodes.Status501NotImplemented); // TODO(fpion): implement
  }

  [HttpPut]
  public async Task<ActionResult<World>> ReplaceAsync(Guid id, [FromBody] ReplaceWorldPayload payload, long? version, CancellationToken cancellationToken)
  {
    await Task.Delay(1, cancellationToken);
    return StatusCode(StatusCodes.Status501NotImplemented); // TODO(fpion): implement
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<World>>> SearchAsync(CancellationToken cancellationToken)
  {
    await Task.Delay(1, cancellationToken);
    return StatusCode(StatusCodes.Status501NotImplemented); // TODO(fpion): implement
  }

  [HttpPatch]
  public async Task<ActionResult<World>> UpdateAsync(Guid id, [FromBody] UpdateWorldPayload payload, CancellationToken cancellationToken)
  {
    await Task.Delay(1, cancellationToken);
    return StatusCode(StatusCodes.Status501NotImplemented); // TODO(fpion): implement
  }
}
