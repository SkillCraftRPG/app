using Logitar.Portal.Contracts.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillCraft.Application;
using SkillCraft.Application.Parties.Commands;
using SkillCraft.Application.Parties.Queries;
using SkillCraft.Constants;
using SkillCraft.Contracts.Parties;
using SkillCraft.Extensions;
using SkillCraft.Filters;
using SkillCraft.Models.Parties;

namespace SkillCraft.Controllers;

[ApiController]
[Authorize]
[RequireWorld]
[Route(Routes.Party)]
public class PartyController : ControllerBase
{
  private readonly IRequestPipeline _pipeline;

  public PartyController(IRequestPipeline pipeline)
  {
    _pipeline = pipeline;
  }

  [HttpPost]
  public async Task<ActionResult<PartyModel>> CreateAsync([FromBody] CreateOrReplacePartyPayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplacePartyResult result = await _pipeline.ExecuteAsync(new CreateOrReplacePartyCommand(Id: null, payload, Version: null), cancellationToken);
    return GetActionResult(result);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<PartyModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    PartyModel? party = await _pipeline.ExecuteAsync(new ReadPartyQuery(id), cancellationToken);
    return GetActionResult(party);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<PartyModel>> ReplaceAsync(Guid id, [FromBody] CreateOrReplacePartyPayload payload, long? version, CancellationToken cancellationToken)
  {
    CreateOrReplacePartyResult result = await _pipeline.ExecuteAsync(new CreateOrReplacePartyCommand(id, payload, version), cancellationToken);
    return GetActionResult(result);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<PartyModel>>> SearchAsync([FromQuery] SearchPartiesParameters parameters, CancellationToken cancellationToken)
  {
    SearchResults<PartyModel> parties = await _pipeline.ExecuteAsync(new SearchPartiesQuery(parameters.ToPayload()), cancellationToken);
    return Ok(parties);
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<PartyModel>> UpdateAsync(Guid id, [FromBody] UpdatePartyPayload payload, CancellationToken cancellationToken)
  {
    PartyModel? party = await _pipeline.ExecuteAsync(new UpdatePartyCommand(id, payload), cancellationToken);
    return GetActionResult(party);
  }

  private ActionResult<PartyModel> GetActionResult(CreateOrReplacePartyResult result) => GetActionResult(result.Party, result.Created);
  private ActionResult<PartyModel> GetActionResult(PartyModel? party, bool created = false)
  {
    if (party == null)
    {
      return NotFound();
    }
    if (created)
    {
      Uri location = HttpContext.BuildLocation($"{Routes.Party}/{{id}}", [new KeyValuePair<string, string>("id", party.Id.ToString())]);
      return Created(location, party);
    }

    return Ok(party);
  }
}
