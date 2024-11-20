using Logitar.Portal.Contracts.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillCraft.Application;
using SkillCraft.Application.Natures.Commands;
using SkillCraft.Application.Natures.Queries;
using SkillCraft.Constants;
using SkillCraft.Contracts.Natures;
using SkillCraft.Extensions;
using SkillCraft.Filters;
using SkillCraft.Models.Natures;

namespace SkillCraft.Controllers;

[ApiController]
[Authorize]
[RequireWorld]
[Route(Routes.Nature)]
public class NatureController : ControllerBase
{
  private readonly IRequestPipeline _pipeline;

  public NatureController(IRequestPipeline pipeline)
  {
    _pipeline = pipeline;
  }

  [HttpPost]
  public async Task<ActionResult<NatureModel>> CreateAsync([FromBody] CreateOrReplaceNaturePayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceNatureResult result = await _pipeline.ExecuteAsync(new CreateOrReplaceNatureCommand(Id: null, payload, Version: null), cancellationToken);
    return GetActionResult(result);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<NatureModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    NatureModel? nature = await _pipeline.ExecuteAsync(new ReadNatureQuery(id), cancellationToken);
    return GetActionResult(nature);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<NatureModel>> ReplaceAsync(Guid id, [FromBody] CreateOrReplaceNaturePayload payload, long? version, CancellationToken cancellationToken)
  {
    CreateOrReplaceNatureResult result = await _pipeline.ExecuteAsync(new CreateOrReplaceNatureCommand(id, payload, version), cancellationToken);
    return GetActionResult(result);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<NatureModel>>> SearchAsync([FromQuery] SearchNaturesParameters parameters, CancellationToken cancellationToken)
  {
    SearchResults<NatureModel> natures = await _pipeline.ExecuteAsync(new SearchNaturesQuery(parameters.ToPayload()), cancellationToken);
    return Ok(natures);
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<NatureModel>> UpdateAsync(Guid id, [FromBody] UpdateNaturePayload payload, CancellationToken cancellationToken)
  {
    NatureModel? nature = await _pipeline.ExecuteAsync(new UpdateNatureCommand(id, payload), cancellationToken);
    return GetActionResult(nature);
  }

  private ActionResult<NatureModel> GetActionResult(CreateOrReplaceNatureResult result) => GetActionResult(result.Nature, result.Created);
  private ActionResult<NatureModel> GetActionResult(NatureModel? nature, bool created = false)
  {
    if (nature == null)
    {
      return NotFound();
    }
    if (created)
    {
      Uri location = HttpContext.BuildLocation($"{Routes.Nature}/{{id}}", [new KeyValuePair<string, string>("id", nature.Id.ToString())]);
      return Created(location, nature);
    }

    return Ok(nature);
  }
}
