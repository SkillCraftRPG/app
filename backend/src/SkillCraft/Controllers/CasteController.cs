using Logitar.Portal.Contracts.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillCraft.Application;
using SkillCraft.Application.Castes.Commands;
using SkillCraft.Application.Castes.Queries;
using SkillCraft.Constants;
using SkillCraft.Contracts.Castes;
using SkillCraft.Extensions;
using SkillCraft.Filters;
using SkillCraft.Models.Castes;

namespace SkillCraft.Controllers;

[ApiController]
[Authorize]
[RequireWorld]
[Route(Routes.Caste)]
public class CasteController : ControllerBase
{
  private readonly IRequestPipeline _pipeline;

  public CasteController(IRequestPipeline pipeline)
  {
    _pipeline = pipeline;
  }

  [HttpPost]
  public async Task<ActionResult<CasteModel>> CreateAsync([FromBody] CreateCastePayload payload, CancellationToken cancellationToken)
  {
    CasteModel caste = await _pipeline.ExecuteAsync(new CreateCasteCommand(payload), cancellationToken);
    Uri location = HttpContext.BuildLocation($"{Routes.Caste}/{{id}}", [new KeyValuePair<string, string>("id", caste.Id.ToString())]);

    return Created(location, caste);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<CasteModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    CasteModel? caste = await _pipeline.ExecuteAsync(new ReadCasteQuery(id), cancellationToken);
    return caste == null ? NotFound() : Ok(caste);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<CasteModel>> ReplaceAsync(Guid id, [FromBody] ReplaceCastePayload payload, long? version, CancellationToken cancellationToken)
  {
    CasteModel? caste = await _pipeline.ExecuteAsync(new ReplaceCasteCommand(id, payload, version), cancellationToken);
    return caste == null ? NotFound() : Ok(caste);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<CasteModel>>> SearchAsync([FromQuery] SearchCastesParameters parameters, CancellationToken cancellationToken)
  {
    SearchResults<CasteModel> worlds = await _pipeline.ExecuteAsync(new SearchCastesQuery(parameters.ToPayload()), cancellationToken);
    return Ok(worlds);
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<CasteModel>> UpdateAsync(Guid id, [FromBody] UpdateCastePayload payload, CancellationToken cancellationToken)
  {
    CasteModel? caste = await _pipeline.ExecuteAsync(new UpdateCasteCommand(id, payload), cancellationToken);
    return caste == null ? NotFound() : Ok(caste);
  }
}
