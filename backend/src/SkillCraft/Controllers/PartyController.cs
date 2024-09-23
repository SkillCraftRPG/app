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
  public async Task<ActionResult<PartyModel>> CreateAsync([FromBody] CreatePartyPayload payload, CancellationToken cancellationToken)
  {
    PartyModel party = await _pipeline.ExecuteAsync(new CreatePartyCommand(payload), cancellationToken);
    Uri location = HttpContext.BuildLocation($"{Routes.Party}/{{id}}", [new KeyValuePair<string, string>("id", party.Id.ToString())]);

    return Created(location, party);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<PartyModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    PartyModel? party = await _pipeline.ExecuteAsync(new ReadPartyQuery(id), cancellationToken);
    return party == null ? NotFound() : Ok(party);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<PartyModel>> ReplaceAsync(Guid id, [FromBody] ReplacePartyPayload payload, long? version, CancellationToken cancellationToken)
  {
    PartyModel? party = await _pipeline.ExecuteAsync(new ReplacePartyCommand(id, payload, version), cancellationToken);
    return party == null ? NotFound() : Ok(party);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<PartyModel>>> SearchAsync([FromQuery] SearchPartiesParameters parameters, CancellationToken cancellationToken)
  {
    SearchResults<PartyModel> worlds = await _pipeline.ExecuteAsync(new SearchPartiesQuery(parameters.ToPayload()), cancellationToken);
    return Ok(worlds);
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<PartyModel>> UpdateAsync(Guid id, [FromBody] UpdatePartyPayload payload, CancellationToken cancellationToken)
  {
    PartyModel? party = await _pipeline.ExecuteAsync(new UpdatePartyCommand(id, payload), cancellationToken);
    return party == null ? NotFound() : Ok(party);
  }
}
