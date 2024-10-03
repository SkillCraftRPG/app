using Logitar.Portal.Contracts.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillCraft.Application;
using SkillCraft.Application.Worlds.Commands;
using SkillCraft.Application.Worlds.Queries;
using SkillCraft.Constants;
using SkillCraft.Contracts.Worlds;
using SkillCraft.Extensions;
using SkillCraft.Models.Worlds;

namespace SkillCraft.Controllers;

[ApiController]
[Authorize]
[Route(Routes.World)]
public class WorldController : ControllerBase
{
  private readonly IRequestPipeline _pipeline;

  public WorldController(IRequestPipeline pipeline)
  {
    _pipeline = pipeline;
  }

  [HttpPost]
  public async Task<ActionResult<WorldModel>> CreateAsync([FromBody] CreateOrReplaceWorldPayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceWorldResult result = await _pipeline.ExecuteAsync(new CreateOrReplaceWorldCommand(Id: null, payload, Version: null), cancellationToken);
    return GetActionResult(result);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<WorldModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    WorldModel? world = await _pipeline.ExecuteAsync(new ReadWorldQuery(id, Slug: null), cancellationToken);
    return GetActionResult(world);
  }

  [HttpGet("slug:{slug}")]
  public async Task<ActionResult<WorldModel>> ReadAsync(string slug, CancellationToken cancellationToken)
  {
    WorldModel? world = await _pipeline.ExecuteAsync(new ReadWorldQuery(Id: null, slug), cancellationToken);
    return GetActionResult(world);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<WorldModel>> ReplaceAsync(Guid id, [FromBody] CreateOrReplaceWorldPayload payload, long? version, CancellationToken cancellationToken)
  {
    CreateOrReplaceWorldResult result = await _pipeline.ExecuteAsync(new CreateOrReplaceWorldCommand(id, payload, version), cancellationToken);
    return GetActionResult(result);
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
    return GetActionResult(world);
  }

  private ActionResult<WorldModel> GetActionResult(CreateOrReplaceWorldResult result) => GetActionResult(result.World, result.Created);
  private ActionResult<WorldModel> GetActionResult(WorldModel? world, bool created = false)
  {
    if (world == null)
    {
      return NotFound();
    }
    if (created)
    {
      Uri location = HttpContext.BuildLocation($"{Routes.World}/{{id}}", [new KeyValuePair<string, string>("id", world.Id.ToString())]);
      return Created(location, world);
    }

    return Ok(world);
  }
}
