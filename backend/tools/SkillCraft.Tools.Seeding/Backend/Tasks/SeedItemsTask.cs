using MediatR;
using SkillCraft.Application;
using SkillCraft.Application.Items.Commands;
using SkillCraft.Contracts.Items;

namespace SkillCraft.Tools.Seeding.Backend.Tasks;

internal class SeedItemsTask : SeedingTask
{
  public override string? Description => "Seeds the character items.";
}

internal class SeedItemsTaskHandler : INotificationHandler<SeedItemsTask>
{
  private static readonly JsonSerializerOptions _serializerOptions = new();
  static SeedItemsTaskHandler()
  {
    _serializerOptions.Converters.Add(new JsonStringEnumConverter());
  }

  private readonly ILogger<SeedItemsTaskHandler> _logger;
  private readonly IRequestPipeline _pipeline;

  public SeedItemsTaskHandler(ILogger<SeedItemsTaskHandler> logger, IRequestPipeline pipeline)
  {
    _logger = logger;
    _pipeline = pipeline;
  }

  public async Task Handle(SeedItemsTask _, CancellationToken cancellationToken)
  {
    string json = await File.ReadAllTextAsync("Backend/items.json", Encoding.UTF8, cancellationToken);
    IEnumerable<ItemPayload>? payloads = JsonSerializer.Deserialize<IEnumerable<ItemPayload>>(json, _serializerOptions);
    if (payloads != null)
    {
      foreach (ItemPayload payload in payloads)
      {
        CreateOrReplaceItemCommand command = new(payload.Id, payload, Version: null);
        CreateOrReplaceItemResult result = await _pipeline.ExecuteAsync(command, cancellationToken);
        ItemModel item = result.Item ?? throw new InvalidOperationException("The item model should not be null.");
        string status = result.Created ? "created" : "updated";
        _logger.LogInformation("The item '{Name}' has been {Status} (Id={Id}).", item.Name, status, item.Id);
      }
    }
  }
}
