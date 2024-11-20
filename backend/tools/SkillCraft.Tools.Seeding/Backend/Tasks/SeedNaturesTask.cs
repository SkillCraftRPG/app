using MediatR;
using SkillCraft.Application;
using SkillCraft.Application.Natures.Commands;
using SkillCraft.Contracts.Natures;

namespace SkillCraft.Tools.Seeding.Backend.Tasks;

internal class SeedNaturesTask : SeedingTask
{
  public override string? Description => "Seeds the character natures.";
}

internal class SeedNaturesTaskHandler : INotificationHandler<SeedNaturesTask>
{
  private static readonly JsonSerializerOptions _serializerOptions = new();
  static SeedNaturesTaskHandler()
  {
    _serializerOptions.Converters.Add(new JsonStringEnumConverter());
  }

  private readonly ILogger<SeedNaturesTaskHandler> _logger;
  private readonly IRequestPipeline _pipeline;

  public SeedNaturesTaskHandler(ILogger<SeedNaturesTaskHandler> logger, IRequestPipeline pipeline)
  {
    _logger = logger;
    _pipeline = pipeline;
  }

  public async Task Handle(SeedNaturesTask _, CancellationToken cancellationToken)
  {
    string json = await File.ReadAllTextAsync("Backend/natures.json", Encoding.UTF8, cancellationToken);
    IEnumerable<NaturePayload>? payloads = JsonSerializer.Deserialize<IEnumerable<NaturePayload>>(json, _serializerOptions);
    if (payloads != null)
    {
      foreach (NaturePayload payload in payloads)
      {
        CreateOrReplaceNatureCommand command = new(payload.Id, payload, Version: null);
        CreateOrReplaceNatureResult result = await _pipeline.ExecuteAsync(command, cancellationToken);
        NatureModel nature = result.Nature ?? throw new InvalidOperationException("The nature model should not be null.");
        string status = result.Created ? "created" : "updated";
        _logger.LogInformation("The nature '{Name}' has been {Status} (Id={Id}).", nature.Name, status, nature.Id);
      }
    }
  }
}
