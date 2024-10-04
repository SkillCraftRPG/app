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
  public async Task<ActionResult<TalentModel>> CreateAsync([FromBody] CreateOrReplaceTalentPayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceTalentResult result = await _pipeline.ExecuteAsync(new CreateOrReplaceTalentCommand(Id: null, payload, Version: null), cancellationToken);
    return GetActionResult(result);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<TalentModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    TalentModel? talent = await _pipeline.ExecuteAsync(new ReadTalentQuery(id), cancellationToken);
    return GetActionResult(talent);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<TalentModel>> ReplaceAsync(Guid id, [FromBody] CreateOrReplaceTalentPayload payload, long? version, CancellationToken cancellationToken)
  {
    CreateOrReplaceTalentResult result = await _pipeline.ExecuteAsync(new CreateOrReplaceTalentCommand(id, payload, version), cancellationToken);
    return GetActionResult(result);
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
    return GetActionResult(talent);
  }

  private ActionResult<TalentModel> GetActionResult(CreateOrReplaceTalentResult result) => GetActionResult(result.Talent, result.Created);
  private ActionResult<TalentModel> GetActionResult(TalentModel? talent, bool created = false)
  {
    if (talent == null)
    {
      return NotFound();
    }
    if (created)
    {
      Uri location = HttpContext.BuildLocation($"{Routes.Talent}/{{id}}", [new KeyValuePair<string, string>("id", talent.Id.ToString())]);
      return Created(location, talent);
    }

    return Ok(talent);
  }
}
