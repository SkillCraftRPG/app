using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillCraft.Application;
using SkillCraft.Application.Characters.Commands;
using SkillCraft.Constants;
using SkillCraft.Contracts.Characters;
using SkillCraft.Extensions;
using SkillCraft.Filters;

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
}
