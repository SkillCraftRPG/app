using Logitar.Portal.Contracts.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillCraft.Application;
using SkillCraft.Application.Educations.Commands;
using SkillCraft.Application.Educations.Queries;
using SkillCraft.Contracts.Educations;
using SkillCraft.Extensions;
using SkillCraft.Filters;
using SkillCraft.Models.Educations;

namespace SkillCraft.Controllers;

[ApiController]
[Authorize]
[RequireWorld]
[Route("educations")]
public class EducationController : ControllerBase
{
  private readonly IRequestPipeline _pipeline;

  public EducationController(IRequestPipeline pipeline)
  {
    _pipeline = pipeline;
  }

  [HttpPost]
  public async Task<ActionResult<EducationModel>> CreateAsync([FromBody] CreateEducationPayload payload, CancellationToken cancellationToken)
  {
    EducationModel education = await _pipeline.ExecuteAsync(new CreateEducationCommand(payload), cancellationToken);
    Uri location = HttpContext.BuildLocation("educations/{id}", [new KeyValuePair<string, string>("id", education.Id.ToString())]);

    return Created(location, education);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<EducationModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    EducationModel? education = await _pipeline.ExecuteAsync(new ReadEducationQuery(id), cancellationToken);
    return education == null ? NotFound() : Ok(education);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<EducationModel>> ReplaceAsync(Guid id, [FromBody] ReplaceEducationPayload payload, long? version, CancellationToken cancellationToken)
  {
    EducationModel? education = await _pipeline.ExecuteAsync(new ReplaceEducationCommand(id, payload, version), cancellationToken);
    return education == null ? NotFound() : Ok(education);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<EducationModel>>> SearchAsync([FromQuery] SearchEducationsParameters parameters, CancellationToken cancellationToken)
  {
    SearchResults<EducationModel> worlds = await _pipeline.ExecuteAsync(new SearchEducationsQuery(parameters.ToPayload()), cancellationToken);
    return Ok(worlds);
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<EducationModel>> UpdateAsync(Guid id, [FromBody] UpdateEducationPayload payload, CancellationToken cancellationToken)
  {
    EducationModel? education = await _pipeline.ExecuteAsync(new UpdateEducationCommand(id, payload), cancellationToken);
    return education == null ? NotFound() : Ok(education);
  }
}
