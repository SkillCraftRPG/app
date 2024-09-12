using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillCraft.Application;
using SkillCraft.Application.Educations.Commands;
using SkillCraft.Contracts.Educations;
using SkillCraft.Extensions;

namespace SkillCraft.Controllers;

[ApiController]
[Authorize]
[Route("educations")]
public class EducationController : ControllerBase // TODO(fpion): World(Filter)Attribute
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
}
