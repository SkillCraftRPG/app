using Logitar.Portal.Contracts.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillCraft.Application;
using SkillCraft.Application.Customizations.Commands;
using SkillCraft.Application.Customizations.Queries;
using SkillCraft.Contracts.Customizations;
using SkillCraft.Extensions;
using SkillCraft.Filters;
using SkillCraft.Models.Customizations;

namespace SkillCraft.Controllers;

[ApiController]
[Authorize]
[RequireWorld]
[Route("customizations")]
public class CustomizationController : ControllerBase
{
  private readonly IRequestPipeline _pipeline;

  public CustomizationController(IRequestPipeline pipeline)
  {
    _pipeline = pipeline;
  }

  [HttpPost]
  public async Task<ActionResult<CustomizationModel>> CreateAsync([FromBody] CreateCustomizationPayload payload, CancellationToken cancellationToken)
  {
    CustomizationModel customization = await _pipeline.ExecuteAsync(new CreateCustomizationCommand(payload), cancellationToken);
    Uri location = HttpContext.BuildLocation("customizations/{id}", [new KeyValuePair<string, string>("id", customization.Id.ToString())]);

    return Created(location, customization);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<CustomizationModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    CustomizationModel? customization = await _pipeline.ExecuteAsync(new ReadCustomizationQuery(id), cancellationToken);
    return customization == null ? NotFound() : Ok(customization);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<CustomizationModel>> ReplaceAsync(Guid id, [FromBody] ReplaceCustomizationPayload payload, long? version, CancellationToken cancellationToken)
  {
    CustomizationModel? customization = await _pipeline.ExecuteAsync(new ReplaceCustomizationCommand(id, payload, version), cancellationToken);
    return customization == null ? NotFound() : Ok(customization);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<CustomizationModel>>> SearchAsync([FromQuery] SearchCustomizationsParameters parameters, CancellationToken cancellationToken)
  {
    SearchResults<CustomizationModel> worlds = await _pipeline.ExecuteAsync(new SearchCustomizationsQuery(parameters.ToPayload()), cancellationToken);
    return Ok(worlds);
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<CustomizationModel>> UpdateAsync(Guid id, [FromBody] UpdateCustomizationPayload payload, CancellationToken cancellationToken)
  {
    CustomizationModel? customization = await _pipeline.ExecuteAsync(new UpdateCustomizationCommand(id, payload), cancellationToken);
    return customization == null ? NotFound() : Ok(customization);
  }
}
