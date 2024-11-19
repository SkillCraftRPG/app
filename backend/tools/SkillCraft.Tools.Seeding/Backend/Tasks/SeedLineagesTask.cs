using MediatR;
using SkillCraft.Application;
using SkillCraft.Application.Lineages.Commands;
using SkillCraft.Contracts.Lineages;

namespace SkillCraft.Tools.Seeding.Backend.Tasks;

internal class SeedLineagesTask : SeedingTask
{
  public override string? Description => "Seeds the character lineages.";
}

internal class SeedLineagesTaskHandler : INotificationHandler<SeedLineagesTask>
{
  private static readonly JsonSerializerOptions _serializerOptions = new();
  static SeedLineagesTaskHandler()
  {
    _serializerOptions.Converters.Add(new JsonStringEnumConverter());
  }

  private readonly ILogger<SeedLineagesTaskHandler> _logger;
  private readonly IRequestPipeline _pipeline;

  public SeedLineagesTaskHandler(ILogger<SeedLineagesTaskHandler> logger, IRequestPipeline pipeline)
  {
    _logger = logger;
    _pipeline = pipeline;
  }

  public async Task Handle(SeedLineagesTask _, CancellationToken cancellationToken)
  {
    string json = await File.ReadAllTextAsync("Backend/lineages.json", Encoding.UTF8, cancellationToken);
    IEnumerable<LineagePayload>? payloads = JsonSerializer.Deserialize<IEnumerable<LineagePayload>>(json, _serializerOptions);
    if (payloads != null)
    {
      foreach (LineagePayload payload in payloads)
      {
        CreateOrReplaceLineageCommand command = new(payload.Id, payload, Version: null);
        CreateOrReplaceLineageResult result = await _pipeline.ExecuteAsync(command, cancellationToken);
        LineageModel lineage = result.Lineage ?? throw new InvalidOperationException("The lineage model should not be null.");
        string status = result.Created ? "created" : "updated";
        _logger.LogInformation("The lineage '{Name}' has been {Status} (Id={Id}).", lineage.Name, status, lineage.Id);
      }
    }
  }
}
