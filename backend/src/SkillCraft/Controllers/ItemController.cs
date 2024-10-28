using Logitar.Portal.Contracts.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillCraft.Application;
using SkillCraft.Application.Items.Commands;
using SkillCraft.Application.Items.Queries;
using SkillCraft.Constants;
using SkillCraft.Contracts.Items;
using SkillCraft.Extensions;
using SkillCraft.Filters;
using SkillCraft.Models.Items;

namespace SkillCraft.Controllers;

[ApiController]
[Authorize]
[RequireWorld]
[Route(Routes.Item)]
public class ItemController : ControllerBase
{
  private readonly IRequestPipeline _pipeline;

  public ItemController(IRequestPipeline pipeline)
  {
    _pipeline = pipeline;
  }

  [HttpPost]
  public async Task<ActionResult<ItemModel>> CreateAsync([FromBody] CreateOrReplaceItemPayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceItemResult result = await _pipeline.ExecuteAsync(new CreateOrReplaceItemCommand(Id: null, payload, Version: null), cancellationToken);
    return GetActionResult(result);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<ItemModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    ItemModel? item = await _pipeline.ExecuteAsync(new ReadItemQuery(id), cancellationToken);
    return GetActionResult(item);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<ItemModel>> ReplaceAsync(Guid id, [FromBody] CreateOrReplaceItemPayload payload, long? version, CancellationToken cancellationToken)
  {
    CreateOrReplaceItemResult result = await _pipeline.ExecuteAsync(new CreateOrReplaceItemCommand(id, payload, version), cancellationToken);
    return GetActionResult(result);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<ItemModel>>> SearchAsync([FromQuery] SearchItemsParameters parameters, CancellationToken cancellationToken)
  {
    SearchResults<ItemModel> items = await _pipeline.ExecuteAsync(new SearchItemsQuery(parameters.ToPayload()), cancellationToken);
    return Ok(items);
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<ItemModel>> UpdateAsync(Guid id, [FromBody] UpdateItemPayload payload, CancellationToken cancellationToken)
  {
    ItemModel? item = await _pipeline.ExecuteAsync(new UpdateItemCommand(id, payload), cancellationToken);
    return GetActionResult(item);
  }

  private ActionResult<ItemModel> GetActionResult(CreateOrReplaceItemResult result) => GetActionResult(result.Item, result.Created);
  private ActionResult<ItemModel> GetActionResult(ItemModel? item, bool created = false)
  {
    if (item == null)
    {
      return NotFound();
    }
    if (created)
    {
      Uri location = HttpContext.BuildLocation($"{Routes.Item}/{{id}}", [new KeyValuePair<string, string>("id", item.Id.ToString())]);
      return Created(location, item);
    }

    return Ok(item);
  }
}
