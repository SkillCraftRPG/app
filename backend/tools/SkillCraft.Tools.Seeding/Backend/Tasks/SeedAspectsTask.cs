using MediatR;
using SkillCraft.Application;
using SkillCraft.Application.Aspects.Commands;
using SkillCraft.Contracts.Aspects;

namespace SkillCraft.Tools.Seeding.Backend.Tasks;

internal class SeedAspectsTask : SeedingTask
{
  public override string? Description => "Seeds the character aspects.";
}

internal class SeedAspectsTaskHandler : INotificationHandler<SeedAspectsTask>
{
  private static readonly JsonSerializerOptions _serializerOptions = new();
  static SeedAspectsTaskHandler()
  {
    _serializerOptions.Converters.Add(new JsonStringEnumConverter());
  }

  private readonly ILogger<SeedAspectsTaskHandler> _logger;
  private readonly IRequestPipeline _pipeline;

  public SeedAspectsTaskHandler(ILogger<SeedAspectsTaskHandler> logger, IRequestPipeline pipeline)
  {
    _logger = logger;
    _pipeline = pipeline;
  }

  public async Task Handle(SeedAspectsTask _, CancellationToken cancellationToken)
  {
    string json = await File.ReadAllTextAsync("Backend/aspects.json", Encoding.UTF8, cancellationToken);
    IEnumerable<AspectPayload>? payloads = JsonSerializer.Deserialize<IEnumerable<AspectPayload>>(json, _serializerOptions);
    if (payloads != null)
    {
      foreach (AspectPayload payload in payloads)
      {
        CreateOrReplaceAspectCommand command = new(payload.Id, payload, Version: null);
        CreateOrReplaceAspectResult result = await _pipeline.ExecuteAsync(command, cancellationToken);
        AspectModel aspect = result.Aspect ?? throw new InvalidOperationException("The aspect model should not be null.");
        string status = result.Created ? "created" : "updated";
        _logger.LogInformation("The aspect '{Name}' has been {Status} (Id={Id}).", aspect.Name, status, aspect.Id);
      }
    }
  }
}
