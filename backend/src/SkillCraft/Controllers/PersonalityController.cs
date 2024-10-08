﻿using Logitar.Portal.Contracts.Search;
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
  public async Task<ActionResult<PersonalityModel>> CreateAsync([FromBody] CreateOrReplacePersonalityPayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplacePersonalityResult result = await _pipeline.ExecuteAsync(new CreateOrReplacePersonalityCommand(Id: null, payload, Version: null), cancellationToken);
    return GetActionResult(result);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<PersonalityModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    PersonalityModel? personality = await _pipeline.ExecuteAsync(new ReadPersonalityQuery(id), cancellationToken);
    return GetActionResult(personality);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<PersonalityModel>> ReplaceAsync(Guid id, [FromBody] CreateOrReplacePersonalityPayload payload, long? version, CancellationToken cancellationToken)
  {
    CreateOrReplacePersonalityResult result = await _pipeline.ExecuteAsync(new CreateOrReplacePersonalityCommand(id, payload, version), cancellationToken);
    return GetActionResult(result);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<PersonalityModel>>> SearchAsync([FromQuery] SearchPersonalitiesParameters parameters, CancellationToken cancellationToken)
  {
    SearchResults<PersonalityModel> personalities = await _pipeline.ExecuteAsync(new SearchPersonalitiesQuery(parameters.ToPayload()), cancellationToken);
    return Ok(personalities);
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<PersonalityModel>> UpdateAsync(Guid id, [FromBody] UpdatePersonalityPayload payload, CancellationToken cancellationToken)
  {
    PersonalityModel? personality = await _pipeline.ExecuteAsync(new UpdatePersonalityCommand(id, payload), cancellationToken);
    return GetActionResult(personality);
  }

  private ActionResult<PersonalityModel> GetActionResult(CreateOrReplacePersonalityResult result) => GetActionResult(result.Personality, result.Created);
  private ActionResult<PersonalityModel> GetActionResult(PersonalityModel? personality, bool created = false)
  {
    if (personality == null)
    {
      return NotFound();
    }
    if (created)
    {
      Uri location = HttpContext.BuildLocation($"{Routes.Personality}/{{id}}", [new KeyValuePair<string, string>("id", personality.Id.ToString())]);
      return Created(location, personality);
    }

    return Ok(personality);
  }
}
