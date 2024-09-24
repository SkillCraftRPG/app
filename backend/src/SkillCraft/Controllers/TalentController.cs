using Logitar.Portal.Contracts.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillCraft.Application;
using SkillCraft.Application.Talents.Commands;
using SkillCraft.Application.Talents.Queries;
using SkillCraft.Constants;
using SkillCraft.Contracts.Talents;
using SkillCraft.Extensions;
using SkillCraft.Filters;
using SkillCraft.Models.Talents;

namespace SkillCraft.Controllers;

[ApiController]
[Authorize]
[RequireWorld]
[Route(Routes.Talent)]
public class TalentController : ControllerBase
{
  private readonly IRequestPipeline _pipeline;

  public TalentController(IRequestPipeline pipeline)
  {
    _pipeline = pipeline;
  }

  [HttpPost]
  public async Task<ActionResult<TalentModel>> CreateAsync([FromBody] CreateTalentPayload payload, CancellationToken cancellationToken)
  {
    TalentModel talent = await _pipeline.ExecuteAsync(new CreateTalentCommand(payload), cancellationToken);
    Uri location = HttpContext.BuildLocation($"{Routes.Talent}/{{id}}", [new KeyValuePair<string, string>("id", talent.Id.ToString())]);

    return Created(location, talent);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<TalentModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    TalentModel? talent = await _pipeline.ExecuteAsync(new ReadTalentQuery(id), cancellationToken);
    return talent == null ? NotFound() : Ok(talent);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<TalentModel>> ReplaceAsync(Guid id, [FromBody] ReplaceTalentPayload payload, long? version, CancellationToken cancellationToken)
  {
    TalentModel? talent = await _pipeline.ExecuteAsync(new ReplaceTalentCommand(id, payload, version), cancellationToken);
    return talent == null ? NotFound() : Ok(talent);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<TalentModel>>> SearchAsync([FromQuery] SearchTalentsParameters parameters, CancellationToken cancellationToken)
  {
    SearchResults<TalentModel> talents = await _pipeline.ExecuteAsync(new SearchTalentsQuery(parameters.ToPayload()), cancellationToken);
    return Ok(talents);
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<TalentModel>> UpdateAsync(Guid id, [FromBody] UpdateTalentPayload payload, CancellationToken cancellationToken)
  {
    TalentModel? talent = await _pipeline.ExecuteAsync(new UpdateTalentCommand(id, payload), cancellationToken);
    return talent == null ? NotFound() : Ok(talent);
  }
}
