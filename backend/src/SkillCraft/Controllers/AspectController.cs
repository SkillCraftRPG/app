using Logitar.Portal.Contracts.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillCraft.Application;
using SkillCraft.Application.Aspects.Commands;
using SkillCraft.Application.Aspects.Queries;
using SkillCraft.Contracts.Aspects;
using SkillCraft.Extensions;
using SkillCraft.Filters;
using SkillCraft.Models.Aspects;

namespace SkillCraft.Controllers;

[ApiController]
[Authorize]
[RequireWorld]
[Route("aspects")]
public class AspectController : ControllerBase
{
  private readonly IRequestPipeline _pipeline;

  public AspectController(IRequestPipeline pipeline)
  {
    _pipeline = pipeline;
  }

  [HttpPost]
  public async Task<ActionResult<AspectModel>> CreateAsync([FromBody] CreateAspectPayload payload, CancellationToken cancellationToken)
  {
    AspectModel aspect = await _pipeline.ExecuteAsync(new CreateAspectCommand(payload), cancellationToken);
    Uri location = HttpContext.BuildLocation("aspects/{id}", [new KeyValuePair<string, string>("id", aspect.Id.ToString())]);

    return Created(location, aspect);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<AspectModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    AspectModel? aspect = await _pipeline.ExecuteAsync(new ReadAspectQuery(id), cancellationToken);
    return aspect == null ? NotFound() : Ok(aspect);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<AspectModel>> ReplaceAsync(Guid id, [FromBody] ReplaceAspectPayload payload, long? version, CancellationToken cancellationToken)
  {
    AspectModel? aspect = await _pipeline.ExecuteAsync(new ReplaceAspectCommand(id, payload, version), cancellationToken);
    return aspect == null ? NotFound() : Ok(aspect);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<AspectModel>>> SearchAsync([FromQuery] SearchAspectsParameters parameters, CancellationToken cancellationToken)
  {
    SearchResults<AspectModel> worlds = await _pipeline.ExecuteAsync(new SearchAspectsQuery(parameters.ToPayload()), cancellationToken);
    return Ok(worlds);
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<AspectModel>> UpdateAsync(Guid id, [FromBody] UpdateAspectPayload payload, CancellationToken cancellationToken)
  {
    AspectModel? aspect = await _pipeline.ExecuteAsync(new UpdateAspectCommand(id, payload), cancellationToken);
    return aspect == null ? NotFound() : Ok(aspect);
  }
}
