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
  private readonly IRequestPipeline _pipeline;

  public WorldController(IRequestPipeline pipeline)
  {
    _pipeline = pipeline;
  }

  [HttpPost]
  public async Task<ActionResult<WorldModel>> CreateAsync([FromBody] CreateWorldPayload payload, CancellationToken cancellationToken)
  {
    WorldModel world = await _pipeline.ExecuteAsync(new CreateWorldCommand(payload), cancellationToken);
    Uri location = HttpContext.BuildLocation("worlds/{id}", [new KeyValuePair<string, string>("id", world.Id.ToString())]);

    return Created(location, world);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<WorldModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    WorldModel? world = await _pipeline.ExecuteAsync(new ReadWorldQuery(id, Slug: null), cancellationToken);
    return world == null ? NotFound() : Ok(world);
  }

  [HttpGet("slug:{slug}")]
  public async Task<ActionResult<WorldModel>> ReadAsync(string slug, CancellationToken cancellationToken)
  {
    WorldModel? world = await _pipeline.ExecuteAsync(new ReadWorldQuery(Id: null, slug), cancellationToken);
    return world == null ? NotFound() : Ok(world);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<WorldModel>> ReplaceAsync(Guid id, [FromBody] ReplaceWorldPayload payload, long? version, CancellationToken cancellationToken)
  {
    WorldModel? world = await _pipeline.ExecuteAsync(new ReplaceWorldCommand(id, payload, version), cancellationToken);
    return world == null ? NotFound() : Ok(world);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<WorldModel>>> SearchAsync([FromQuery] SearchWorldsParameters parameters, CancellationToken cancellationToken)
  {
    SearchResults<WorldModel> worlds = await _pipeline.ExecuteAsync(new SearchWorldsQuery(parameters.ToPayload()), cancellationToken);
    return Ok(worlds);
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<WorldModel>> UpdateAsync(Guid id, [FromBody] UpdateWorldPayload payload, CancellationToken cancellationToken)
  {
    WorldModel? world = await _pipeline.ExecuteAsync(new UpdateWorldCommand(id, payload), cancellationToken);
    return world == null ? NotFound() : Ok(world);
  }
}
