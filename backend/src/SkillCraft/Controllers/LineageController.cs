using Logitar.Portal.Contracts.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillCraft.Application;
using SkillCraft.Application.Lineages.Commands;
using SkillCraft.Application.Lineages.Queries;
using SkillCraft.Contracts.Lineages;
using SkillCraft.Extensions;
using SkillCraft.Filters;
using SkillCraft.Models.Lineages;

namespace SkillCraft.Controllers;

[ApiController]
[Authorize]
[RequireWorld]
[Route("lineages")]
public class LineageController : ControllerBase
{
  private readonly IRequestPipeline _pipeline;

  public LineageController(IRequestPipeline pipeline)
  {
    _pipeline = pipeline;
  }

  [HttpPost]
  public async Task<ActionResult<LineageModel>> CreateAsync([FromBody] CreateLineagePayload payload, CancellationToken cancellationToken)
  {
    LineageModel lineage = await _pipeline.ExecuteAsync(new CreateLineageCommand(payload), cancellationToken);
    Uri location = HttpContext.BuildLocation("lineages/{id}", [new KeyValuePair<string, string>("id", lineage.Id.ToString())]);

    return Created(location, lineage);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<LineageModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    LineageModel? lineage = await _pipeline.ExecuteAsync(new ReadLineageQuery(id), cancellationToken);
    return lineage == null ? NotFound() : Ok(lineage);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<LineageModel>> ReplaceAsync(Guid id, [FromBody] ReplaceLineagePayload payload, long? version, CancellationToken cancellationToken)
  {
    LineageModel? lineage = await _pipeline.ExecuteAsync(new ReplaceLineageCommand(id, payload, version), cancellationToken);
    return lineage == null ? NotFound() : Ok(lineage);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<LineageModel>>> SearchAsync([FromQuery] SearchLineagesParameters parameters, CancellationToken cancellationToken)
  {
    SearchResults<LineageModel> worlds = await _pipeline.ExecuteAsync(new SearchLineagesQuery(parameters.ToPayload()), cancellationToken);
    return Ok(worlds);
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<LineageModel>> UpdateAsync(Guid id, [FromBody] UpdateLineagePayload payload, CancellationToken cancellationToken)
  {
    LineageModel? lineage = await _pipeline.ExecuteAsync(new UpdateLineageCommand(id, payload), cancellationToken);
    return lineage == null ? NotFound() : Ok(lineage);
  }
}
