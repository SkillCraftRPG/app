using Logitar.Portal.Contracts.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillCraft.Application;
using SkillCraft.Application.Lineages.Commands;
using SkillCraft.Application.Lineages.Queries;
using SkillCraft.Constants;
using SkillCraft.Contracts.Lineages;
using SkillCraft.Extensions;
using SkillCraft.Filters;
using SkillCraft.Models.Lineages;

namespace SkillCraft.Controllers;

[ApiController]
[Authorize]
[RequireWorld]
[Route(Routes.Lineage)]
public class LineageController : ControllerBase
{
  private readonly IRequestPipeline _pipeline;

  public LineageController(IRequestPipeline pipeline)
  {
    _pipeline = pipeline;
  }

  [HttpPost]
  public async Task<ActionResult<LineageModel>> CreateAsync([FromBody] CreateOrReplaceLineagePayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceLineageResult result = await _pipeline.ExecuteAsync(new CreateOrReplaceLineageCommand(Id: null, payload, Version: null), cancellationToken);
    return GetActionResult(result);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<LineageModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    LineageModel? lineage = await _pipeline.ExecuteAsync(new ReadLineageQuery(id), cancellationToken);
    return GetActionResult(lineage);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<LineageModel>> ReplaceAsync(Guid id, [FromBody] CreateOrReplaceLineagePayload payload, long? version, CancellationToken cancellationToken)
  {
    CreateOrReplaceLineageResult result = await _pipeline.ExecuteAsync(new CreateOrReplaceLineageCommand(id, payload, version), cancellationToken);
    return GetActionResult(result);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<LineageModel>>> SearchAsync([FromQuery] SearchLineagesParameters parameters, CancellationToken cancellationToken)
  {
    SearchResults<LineageModel> lineages = await _pipeline.ExecuteAsync(new SearchLineagesQuery(parameters.ToPayload()), cancellationToken);
    return Ok(lineages);
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<LineageModel>> UpdateAsync(Guid id, [FromBody] UpdateLineagePayload payload, CancellationToken cancellationToken)
  {
    LineageModel? lineage = await _pipeline.ExecuteAsync(new UpdateLineageCommand(id, payload), cancellationToken);
    return GetActionResult(lineage);
  }

  private ActionResult<LineageModel> GetActionResult(CreateOrReplaceLineageResult result) => GetActionResult(result.Lineage, result.Created);
  private ActionResult<LineageModel> GetActionResult(LineageModel? lineage, bool created = false)
  {
    if (lineage == null)
    {
      return NotFound();
    }
    if (created)
    {
      Uri location = HttpContext.BuildLocation($"{Routes.Lineage}/{{id}}", [new KeyValuePair<string, string>("id", lineage.Id.ToString())]);
      return Created(location, lineage);
    }

    return Ok(lineage);
  }
}
