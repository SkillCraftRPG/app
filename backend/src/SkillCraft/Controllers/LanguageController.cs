using Logitar.Portal.Contracts.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillCraft.Application;
using SkillCraft.Application.Languages.Commands;
using SkillCraft.Application.Languages.Queries;
using SkillCraft.Contracts.Languages;
using SkillCraft.Extensions;
using SkillCraft.Filters;
using SkillCraft.Models.Languages;

namespace SkillCraft.Controllers;

[ApiController]
[Authorize]
[RequireWorld]
[Route("languages")]
public class LanguageController : ControllerBase
{
  private readonly IRequestPipeline _pipeline;

  public LanguageController(IRequestPipeline pipeline)
  {
    _pipeline = pipeline;
  }

  [HttpPost]
  public async Task<ActionResult<LanguageModel>> CreateAsync([FromBody] CreateLanguagePayload payload, CancellationToken cancellationToken)
  {
    LanguageModel language = await _pipeline.ExecuteAsync(new CreateLanguageCommand(payload), cancellationToken);
    Uri location = HttpContext.BuildLocation("languages/{id}", [new KeyValuePair<string, string>("id", language.Id.ToString())]);

    return Created(location, language);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<LanguageModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    LanguageModel? language = await _pipeline.ExecuteAsync(new ReadLanguageQuery(id), cancellationToken);
    return language == null ? NotFound() : Ok(language);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<LanguageModel>> ReplaceAsync(Guid id, [FromBody] ReplaceLanguagePayload payload, long? version, CancellationToken cancellationToken)
  {
    LanguageModel? language = await _pipeline.ExecuteAsync(new ReplaceLanguageCommand(id, payload, version), cancellationToken);
    return language == null ? NotFound() : Ok(language);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<LanguageModel>>> SearchAsync([FromQuery] SearchLanguagesParameters parameters, CancellationToken cancellationToken)
  {
    SearchResults<LanguageModel> worlds = await _pipeline.ExecuteAsync(new SearchLanguagesQuery(parameters.ToPayload()), cancellationToken);
    return Ok(worlds);
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<LanguageModel>> UpdateAsync(Guid id, [FromBody] UpdateLanguagePayload payload, CancellationToken cancellationToken)
  {
    LanguageModel? language = await _pipeline.ExecuteAsync(new UpdateLanguageCommand(id, payload), cancellationToken);
    return language == null ? NotFound() : Ok(language);
  }
}
