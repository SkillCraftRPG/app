using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillCraft.Application;
using SkillCraft.Application.Worlds.Commands;
using SkillCraft.Contracts.Worlds;
using SkillCraft.Extensions;

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
}
