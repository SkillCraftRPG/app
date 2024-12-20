﻿using Logitar.Portal.Contracts.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillCraft.Application;
using SkillCraft.Application.Characters.Commands;
using SkillCraft.Application.Characters.Queries;
using SkillCraft.Constants;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Characters;
using SkillCraft.Extensions;
using SkillCraft.Filters;
using SkillCraft.Models.Characters;

namespace SkillCraft.Controllers;

[ApiController]
[Authorize]
[RequireWorld]
[Route(Routes.Character)]
public class CharacterController : ControllerBase
{
  private readonly IRequestPipeline _pipeline;

  public CharacterController(IRequestPipeline pipeline)
  {
    _pipeline = pipeline;
  }

  [HttpPost("{characterId}/bonuses")]
  public async Task<ActionResult<CharacterModel>> AddBonusAsync(Guid characterId, [FromBody] BonusPayload payload, CancellationToken cancellationToken)
  {
    CharacterModel? character = await _pipeline.ExecuteAsync(new SaveCharacterBonusCommand(characterId, BonusId: null, payload), cancellationToken);
    return GetActionResult(character);
  }

  [HttpPost("{characterId}/talents")]
  public async Task<ActionResult<CharacterModel>> AddTalentAsync(Guid characterId, [FromBody] CharacterTalentPayload payload, CancellationToken cancellationToken)
  {
    CharacterModel? character = await _pipeline.ExecuteAsync(new SaveCharacterTalentCommand(characterId, RelationId: null, payload), cancellationToken);
    return GetActionResult(character);
  }

  [HttpDelete("{id}/level-up")]
  public async Task<ActionResult<CharacterModel>> CancelLevelUpAsync(Guid id, CancellationToken cancellationToken)
  {
    CharacterModel? character = await _pipeline.ExecuteAsync(new CancelCharacterLevelUpCommand(id), cancellationToken);
    return GetActionResult(character);
  }

  [HttpPost]
  public async Task<ActionResult<CharacterModel>> CreateAsync([FromBody] CreateCharacterPayload payload, Guid? id, CancellationToken cancellationToken)
  {
    CharacterModel character = await _pipeline.ExecuteAsync(new CreateCharacterCommand(id, payload), cancellationToken);
    return GetActionResult(character, created: true);
  }

  [HttpPatch("{id}/level-up")]
  public async Task<ActionResult<CharacterModel>> LevelUpAsync(Guid id, [FromBody] LevelUpCharacterPayload payload, CancellationToken cancellationToken)
  {
    CharacterModel? character = await _pipeline.ExecuteAsync(new LevelUpCharacterCommand(id, payload), cancellationToken);
    return GetActionResult(character);
  }

  [HttpGet("players")]
  public async Task<ActionResult<SearchResults<string>>> ListPlayersAsync(CancellationToken cancellationToken)
  {
    SearchResults<string> players = await _pipeline.ExecuteAsync(new SearchPlayersQuery(), cancellationToken);
    return Ok(players);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<CharacterModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    CharacterModel? character = await _pipeline.ExecuteAsync(new ReadCharacterQuery(id), cancellationToken);
    return GetActionResult(character);
  }

  [HttpDelete("{characterId}/bonuses/{bonusId}")]
  public async Task<ActionResult<CharacterModel>> RemoveBonusAsync(Guid characterId, Guid bonusId, CancellationToken cancellationToken)
  {
    CharacterModel? character = await _pipeline.ExecuteAsync(new RemoveCharacterBonusCommand(characterId, bonusId), cancellationToken);
    return GetActionResult(character);
  }

  [HttpDelete("{characterId}/languages/{languageId}")]
  public async Task<ActionResult<CharacterModel>> RemoveLanguageAsync(Guid characterId, Guid languageId, CancellationToken cancellationToken)
  {
    CharacterModel? character = await _pipeline.ExecuteAsync(new RemoveCharacterLanguageCommand(characterId, languageId), cancellationToken);
    return GetActionResult(character);
  }

  [HttpPatch("{id}/skills/{skill}/increase-rank")]
  public async Task<ActionResult<CharacterModel>> IncreaseSkillRankAsync(Guid id, Skill skill, CancellationToken cancellationToken)
  {
    CharacterModel? character = await _pipeline.ExecuteAsync(new IncreaseCharacterSkillRankCommand(id, skill), cancellationToken);
    return GetActionResult(character);
  }

  [HttpDelete("{characterId}/talents/{relationId}")]
  public async Task<ActionResult<CharacterModel>> RemoveTalentAsync(Guid characterId, Guid relationId, CancellationToken cancellationToken)
  {
    CharacterModel? character = await _pipeline.ExecuteAsync(new RemoveCharacterTalentCommand(characterId, relationId), cancellationToken);
    return GetActionResult(character);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<CharacterModel>> ReplaceAsync(Guid id, [FromBody] ReplaceCharacterPayload payload, long? version, CancellationToken cancellationToken)
  {
    CharacterModel? character = await _pipeline.ExecuteAsync(new ReplaceCharacterCommand(id, payload, version), cancellationToken);
    return GetActionResult(character);
  }

  [HttpPut("{characterId}/bonuses/{bonusId}")]
  public async Task<ActionResult<CharacterModel>> SaveBonusAsync(Guid characterId, Guid bonusId, [FromBody] BonusPayload payload, CancellationToken cancellationToken)
  {
    CharacterModel? character = await _pipeline.ExecuteAsync(new SaveCharacterBonusCommand(characterId, bonusId, payload), cancellationToken);
    return GetActionResult(character);
  }

  [HttpPut("{characterId}/languages/{languageId}")]
  public async Task<ActionResult<CharacterModel>> SaveLanguageAsync(Guid characterId, Guid languageId, [FromBody] CharacterLanguagePayload payload, CancellationToken cancellationToken)
  {
    CharacterModel? character = await _pipeline.ExecuteAsync(new SaveCharacterLanguageCommand(characterId, languageId, payload), cancellationToken);
    return GetActionResult(character);
  }

  [HttpPut("{characterId}/talents/{relationId}")]
  public async Task<ActionResult<CharacterModel>> SaveTalentAsync(Guid characterId, Guid relationId, [FromBody] CharacterTalentPayload payload, CancellationToken cancellationToken)
  {
    CharacterModel? character = await _pipeline.ExecuteAsync(new SaveCharacterTalentCommand(characterId, relationId, payload), cancellationToken);
    return GetActionResult(character);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<CharacterModel>>> SearchAsync([FromQuery] SearchCharactersParameters parameters, CancellationToken cancellationToken)
  {
    SearchResults<CharacterModel> characters = await _pipeline.ExecuteAsync(new SearchCharactersQuery(parameters.ToPayload()), cancellationToken);
    return Ok(characters);
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<CharacterModel>> UpdateAsync(Guid id, [FromBody] UpdateCharacterPayload payload, CancellationToken cancellationToken)
  {
    CharacterModel? character = await _pipeline.ExecuteAsync(new UpdateCharacterCommand(id, payload), cancellationToken);
    return GetActionResult(character);
  }

  private ActionResult<CharacterModel> GetActionResult(CharacterModel? character, bool created = false)
  {
    if (character == null)
    {
      return NotFound();
    }
    if (created)
    {
      Uri location = HttpContext.BuildLocation($"{Routes.Character}/{{id}}", [new KeyValuePair<string, string>("id", character.Id.ToString())]);
      return Created(location, character);
    }

    return Ok(character);
  }
}
