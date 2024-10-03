using Logitar.Portal.Contracts.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillCraft.Application;
using SkillCraft.Application.Aspects.Commands;
using SkillCraft.Application.Aspects.Queries;
using SkillCraft.Constants;
using SkillCraft.Contracts.Aspects;
using SkillCraft.Extensions;
using SkillCraft.Filters;
using SkillCraft.Models.Aspects;

namespace SkillCraft.Controllers;

[ApiController]
[Authorize]
[RequireWorld]
[Route(Routes.Aspect)]
public class AspectController : ControllerBase
{
  private readonly IRequestPipeline _pipeline;

  public AspectController(IRequestPipeline pipeline)
  {
    _pipeline = pipeline;
  }

  [HttpPost]
  public async Task<ActionResult<AspectModel>> CreateAsync([FromBody] CreateOrReplaceAspectPayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceAspectResult result = await _pipeline.ExecuteAsync(new CreateOrReplaceAspectCommand(Id: null, payload, Version: null), cancellationToken);
    return GetActionResult(result);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<AspectModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    AspectModel? aspect = await _pipeline.ExecuteAsync(new ReadAspectQuery(id), cancellationToken);
    return GetActionResult(aspect);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<AspectModel>> ReplaceAsync(Guid id, [FromBody] CreateOrReplaceAspectPayload payload, long? version, CancellationToken cancellationToken)
  {
    CreateOrReplaceAspectResult result = await _pipeline.ExecuteAsync(new CreateOrReplaceAspectCommand(id, payload, version), cancellationToken);
    return GetActionResult(result);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<AspectModel>>> SearchAsync([FromQuery] SearchAspectsParameters parameters, CancellationToken cancellationToken)
  {
    SearchResults<AspectModel> aspects = await _pipeline.ExecuteAsync(new SearchAspectsQuery(parameters.ToPayload()), cancellationToken);
    return Ok(aspects);
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<AspectModel>> UpdateAsync(Guid id, [FromBody] UpdateAspectPayload payload, CancellationToken cancellationToken)
  {
    AspectModel? aspect = await _pipeline.ExecuteAsync(new UpdateAspectCommand(id, payload), cancellationToken);
    return GetActionResult(aspect);
  }

  private ActionResult<AspectModel> GetActionResult(CreateOrReplaceAspectResult result) => GetActionResult(result.Aspect, result.Created);
  private ActionResult<AspectModel> GetActionResult(AspectModel? aspect, bool created = false)
  {
    if (aspect == null)
    {
      return NotFound();
    }
    if (created)
    {
      Uri location = HttpContext.BuildLocation($"{Routes.Aspect}/{{id}}", [new KeyValuePair<string, string>("id", aspect.Id.ToString())]);
      return Created(location, aspect);
    }

    return Ok(aspect);
  }
}
