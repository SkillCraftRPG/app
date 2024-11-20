using Logitar.Portal.Contracts.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillCraft.Application;
using SkillCraft.Application.Characters.Commands;
using SkillCraft.Application.Characters.Queries;
using SkillCraft.Constants;
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

  [HttpPost]
  public async Task<ActionResult<CharacterModel>> CreateAsync([FromBody] CreateCharacterPayload payload, CancellationToken cancellationToken)
  {
    CharacterModel character = await _pipeline.ExecuteAsync(new CreateCharacterCommand(payload), cancellationToken);
    Uri location = HttpContext.BuildLocation($"{Routes.Character}/{{id}}", [new KeyValuePair<string, string>("id", character.Id.ToString())]);

    return Created(location, character);
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
    return character == null ? NotFound() : Ok(character);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<CharacterModel>>> SearchAsync([FromQuery] SearchCharactersParameters parameters, CancellationToken cancellationToken)
  {
    SearchResults<CharacterModel> castes = await _pipeline.ExecuteAsync(new SearchCharactersQuery(parameters.ToPayload()), cancellationToken);
    return Ok(castes);
  }

  [HttpPut("{characterId}/languages/{languageId}")]
  public async Task<ActionResult<CharacterModel>> SetLanguageAsync(Guid characterId, Guid languageId, [FromBody] CharacterLanguagePayload payload, CancellationToken cancellationToken)
  {
    CharacterModel? character = await _pipeline.ExecuteAsync(new SetCharacterLanguageCommand(characterId, languageId, payload), cancellationToken);
    return character == null ? NotFound() : Ok(character);
  }
}
