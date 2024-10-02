using Logitar.Portal.Contracts.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillCraft.Application;
using SkillCraft.Application.Customizations.Commands;
using SkillCraft.Application.Customizations.Queries;
using SkillCraft.Constants;
using SkillCraft.Contracts.Customizations;
using SkillCraft.Extensions;
using SkillCraft.Filters;
using SkillCraft.Models.Customizations;

namespace SkillCraft.Controllers;

[ApiController]
[Authorize]
[RequireWorld]
[Route(Routes.Customization)]
public class CustomizationController : ControllerBase
{
  private readonly IRequestPipeline _pipeline;

  public CustomizationController(IRequestPipeline pipeline)
  {
    _pipeline = pipeline;
  }

  [HttpPost]
  public async Task<ActionResult<CustomizationModel>> CreateAsync([FromBody] SaveCustomizationPayload payload, CancellationToken cancellationToken)
  {
    SaveCustomizationResult result = await _pipeline.ExecuteAsync(new SaveCustomizationCommand(Id: null, payload, Version: null), cancellationToken);
    return GetActionResult(result);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<CustomizationModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    CustomizationModel? customization = await _pipeline.ExecuteAsync(new ReadCustomizationQuery(id), cancellationToken);
    return GetActionResult(customization);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<CustomizationModel>> ReplaceAsync(Guid id, [FromBody] SaveCustomizationPayload payload, long? version, CancellationToken cancellationToken)
  {
    SaveCustomizationResult result = await _pipeline.ExecuteAsync(new SaveCustomizationCommand(id, payload, version), cancellationToken);
    return GetActionResult(result);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<CustomizationModel>>> SearchAsync([FromQuery] SearchCustomizationsParameters parameters, CancellationToken cancellationToken)
  {
    SearchResults<CustomizationModel> customizations = await _pipeline.ExecuteAsync(new SearchCustomizationsQuery(parameters.ToPayload()), cancellationToken);
    return Ok(customizations);
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<CustomizationModel>> UpdateAsync(Guid id, [FromBody] UpdateCustomizationPayload payload, CancellationToken cancellationToken)
  {
    CustomizationModel? customization = await _pipeline.ExecuteAsync(new UpdateCustomizationCommand(id, payload), cancellationToken);
    return GetActionResult(customization);
  }

  private ActionResult<CustomizationModel> GetActionResult(SaveCustomizationResult result) => GetActionResult(result.Customization, result.Created);
  private ActionResult<CustomizationModel> GetActionResult(CustomizationModel? customization, bool created = false)
  {
    if (customization == null)
    {
      return NotFound();
    }
    if (created)
    {
      Uri location = HttpContext.BuildLocation($"{Routes.Customization}/{{id}}", [new KeyValuePair<string, string>("id", customization.Id.ToString())]);
      return Created(location, customization);
    }

    return Ok(customization);
  }
}
