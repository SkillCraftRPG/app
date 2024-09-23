using Logitar.Portal.Contracts.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillCraft.Application;
using SkillCraft.Application.Personalities.Commands;
using SkillCraft.Application.Personalities.Queries;
using SkillCraft.Constants;
using SkillCraft.Contracts.Personalities;
using SkillCraft.Extensions;
using SkillCraft.Filters;
using SkillCraft.Models.Personalities;

namespace SkillCraft.Controllers;

[ApiController]
[Authorize]
[RequireWorld]
[Route(Routes.Personality)]
public class PersonalityController : ControllerBase
{
  private readonly IRequestPipeline _pipeline;

  public PersonalityController(IRequestPipeline pipeline)
  {
    _pipeline = pipeline;
  }

  [HttpPost]
  public async Task<ActionResult<PersonalityModel>> CreateAsync([FromBody] CreatePersonalityPayload payload, CancellationToken cancellationToken)
  {
    PersonalityModel personality = await _pipeline.ExecuteAsync(new CreatePersonalityCommand(payload), cancellationToken);
    Uri location = HttpContext.BuildLocation($"{Routes.Personality}/{{id}}", [new KeyValuePair<string, string>("id", personality.Id.ToString())]);

    return Created(location, personality);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<PersonalityModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    PersonalityModel? personality = await _pipeline.ExecuteAsync(new ReadPersonalityQuery(id), cancellationToken);
    return personality == null ? NotFound() : Ok(personality);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<PersonalityModel>> ReplaceAsync(Guid id, [FromBody] ReplacePersonalityPayload payload, long? version, CancellationToken cancellationToken)
  {
    PersonalityModel? personality = await _pipeline.ExecuteAsync(new ReplacePersonalityCommand(id, payload, version), cancellationToken);
    return personality == null ? NotFound() : Ok(personality);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<PersonalityModel>>> SearchAsync([FromQuery] SearchPersonalitiesParameters parameters, CancellationToken cancellationToken)
  {
    SearchResults<PersonalityModel> worlds = await _pipeline.ExecuteAsync(new SearchPersonalitiesQuery(parameters.ToPayload()), cancellationToken);
    return Ok(worlds);
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<PersonalityModel>> UpdateAsync(Guid id, [FromBody] UpdatePersonalityPayload payload, CancellationToken cancellationToken)
  {
    PersonalityModel? personality = await _pipeline.ExecuteAsync(new UpdatePersonalityCommand(id, payload), cancellationToken);
    return personality == null ? NotFound() : Ok(personality);
  }
}
