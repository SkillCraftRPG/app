using Logitar.Portal.Contracts.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillCraft.Application;
using SkillCraft.Application.Languages.Commands;
using SkillCraft.Application.Languages.Queries;
using SkillCraft.Constants;
using SkillCraft.Contracts.Languages;
using SkillCraft.Extensions;
using SkillCraft.Filters;
using SkillCraft.Models.Languages;

namespace SkillCraft.Controllers;

[ApiController]
[Authorize]
[RequireWorld]
[Route(Routes.Language)]
public class LanguageController : ControllerBase
{
  private readonly IRequestPipeline _pipeline;

  public LanguageController(IRequestPipeline pipeline)
  {
    _pipeline = pipeline;
  }

  [HttpPost]
  public async Task<ActionResult<LanguageModel>> CreateAsync([FromBody] CreateOrReplaceLanguagePayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceLanguageResult result = await _pipeline.ExecuteAsync(new CreateOrReplaceLanguageCommand(Id: null, payload, Version: null), cancellationToken);
    return GetActionResult(result);
  }

  [HttpGet("scripts")]
  public async Task<ActionResult<SearchResults<string>>> ListScriptsAsync(CancellationToken cancellationToken)
  {
    SearchResults<string> scripts = await _pipeline.ExecuteAsync(new SearchScriptsQuery(), cancellationToken);
    return Ok(scripts);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<LanguageModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    LanguageModel? language = await _pipeline.ExecuteAsync(new ReadLanguageQuery(id), cancellationToken);
    return GetActionResult(language);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<LanguageModel>> ReplaceAsync(Guid id, [FromBody] CreateOrReplaceLanguagePayload payload, long? version, CancellationToken cancellationToken)
  {
    CreateOrReplaceLanguageResult result = await _pipeline.ExecuteAsync(new CreateOrReplaceLanguageCommand(id, payload, version), cancellationToken);
    return GetActionResult(result);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<LanguageModel>>> SearchAsync([FromQuery] SearchLanguagesParameters parameters, CancellationToken cancellationToken)
  {
    SearchResults<LanguageModel> languages = await _pipeline.ExecuteAsync(new SearchLanguagesQuery(parameters.ToPayload()), cancellationToken);
    return Ok(languages);
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<LanguageModel>> UpdateAsync(Guid id, [FromBody] UpdateLanguagePayload payload, CancellationToken cancellationToken)
  {
    LanguageModel? language = await _pipeline.ExecuteAsync(new UpdateLanguageCommand(id, payload), cancellationToken);
    return GetActionResult(language);
  }

  private ActionResult<LanguageModel> GetActionResult(CreateOrReplaceLanguageResult result) => GetActionResult(result.Language, result.Created);
  private ActionResult<LanguageModel> GetActionResult(LanguageModel? language, bool created = false)
  {
    if (language == null)
    {
      return NotFound();
    }
    if (created)
    {
      Uri location = HttpContext.BuildLocation($"{Routes.Language}/{{id}}", [new KeyValuePair<string, string>("id", language.Id.ToString())]);
      return Created(location, language);
    }

    return Ok(language);
  }
}
