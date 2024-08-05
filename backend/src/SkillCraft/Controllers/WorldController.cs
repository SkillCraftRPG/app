using Logitar.Portal.Contracts.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillCraft.Application;
using SkillCraft.Application.Worlds.Commands;
using SkillCraft.Application.Worlds.Queries;
using SkillCraft.Contracts.Worlds;
using SkillCraft.Extensions;
using SkillCraft.Models.Worlds;

namespace SkillCraft.Controllers;

[ApiController]
[Authorize]
[Route("worlds")]
public class WorldController : ControllerBase
{
  private readonly IRequestPipeline _requestPipeline;

  public WorldController(IRequestPipeline requestPipeline)
  {
    _requestPipeline = requestPipeline;
  }

  [HttpPost]
  public async Task<ActionResult<World>> CreateAsync([FromBody] CreateWorldPayload payload, CancellationToken cancellationToken)
  {
    World world = await _requestPipeline.ExecuteAsync(new CreateWorldCommand(payload), cancellationToken);
    Uri uri = HttpContext.BuildLocation("/worlds/{id}", [new("id", world.Id.ToString())]);

    return Created(uri, world);
  }

  [HttpDelete("{id}")]
  public async Task<ActionResult<World>> DeleteAsync(Guid id, CancellationToken cancellationToken)
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
  public async Task<ActionResult<SearchResults<World>>> SearchAsync([FromQuery] SearchWorldsParameters parameters, CancellationToken cancellationToken)
  {
    SearchResults<World> worlds = await _requestPipeline.ExecuteAsync(new SearchWorldsQuery(parameters.ToPayload()), cancellationToken);
    return Ok(worlds);
  }

  [HttpPatch]
  public async Task<ActionResult<World>> UpdateAsync(Guid id, [FromBody] UpdateWorldPayload payload, CancellationToken cancellationToken)
  {
    await Task.Delay(1, cancellationToken);
    return StatusCode(StatusCodes.Status501NotImplemented); // TODO(fpion): implement
  }
}
