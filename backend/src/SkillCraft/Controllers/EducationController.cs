using Logitar.Portal.Contracts.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillCraft.Application;
using SkillCraft.Application.Educations.Commands;
using SkillCraft.Application.Educations.Queries;
using SkillCraft.Constants;
using SkillCraft.Contracts.Educations;
using SkillCraft.Extensions;
using SkillCraft.Filters;
using SkillCraft.Models.Educations;

namespace SkillCraft.Controllers;

[ApiController]
[Authorize]
[RequireWorld]
[Route(Routes.Education)]
public class EducationController : ControllerBase
{
  private readonly IRequestPipeline _pipeline;

  public EducationController(IRequestPipeline pipeline)
  {
    _pipeline = pipeline;
  }

  [HttpPost]
  public async Task<ActionResult<EducationModel>> CreateAsync([FromBody] CreateOrReplaceEducationPayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceEducationResult result = await _pipeline.ExecuteAsync(new CreateOrReplaceEducationCommand(Id: null, payload, Version: null), cancellationToken);
    return GetActionResult(result);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<EducationModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    EducationModel? education = await _pipeline.ExecuteAsync(new ReadEducationQuery(id), cancellationToken);
    return GetActionResult(education);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<EducationModel>> ReplaceAsync(Guid id, [FromBody] CreateOrReplaceEducationPayload payload, long? version, CancellationToken cancellationToken)
  {
    CreateOrReplaceEducationResult result = await _pipeline.ExecuteAsync(new CreateOrReplaceEducationCommand(id, payload, version), cancellationToken);
    return GetActionResult(result);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<EducationModel>>> SearchAsync([FromQuery] SearchEducationsParameters parameters, CancellationToken cancellationToken)
  {
    SearchResults<EducationModel> educations = await _pipeline.ExecuteAsync(new SearchEducationsQuery(parameters.ToPayload()), cancellationToken);
    return Ok(educations);
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<EducationModel>> UpdateAsync(Guid id, [FromBody] UpdateEducationPayload payload, CancellationToken cancellationToken)
  {
    EducationModel? education = await _pipeline.ExecuteAsync(new UpdateEducationCommand(id, payload), cancellationToken);
    return GetActionResult(education);
  }

  private ActionResult<EducationModel> GetActionResult(CreateOrReplaceEducationResult result) => GetActionResult(result.Education, result.Created);
  private ActionResult<EducationModel> GetActionResult(EducationModel? education, bool created = false)
  {
    if (education == null)
    {
      return NotFound();
    }
    if (created)
    {
      Uri location = HttpContext.BuildLocation($"{Routes.Education}/{{id}}", [new KeyValuePair<string, string>("id", education.Id.ToString())]);
      return Created(location, education);
    }

    return Ok(education);
  }
}
