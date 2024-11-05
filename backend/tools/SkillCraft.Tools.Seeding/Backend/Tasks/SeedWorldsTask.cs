using MediatR;
using SkillCraft.Application;
using SkillCraft.Application.Worlds.Commands;
using SkillCraft.Contracts.Worlds;

namespace SkillCraft.Tools.Seeding.Backend.Tasks;

internal class SeedWorldsTask : SeedingTask
{
  public override string? Description => "Seeds the game worlds.";
}

internal class SeedWorldsTaskHandler : INotificationHandler<SeedWorldsTask>
{
  private static readonly JsonSerializerOptions _serializerOptions = new();
  static SeedWorldsTaskHandler()
  {
    _serializerOptions.Converters.Add(new JsonStringEnumConverter());
  }

  private readonly ILogger<SeedWorldsTaskHandler> _logger;
  private readonly IRequestPipeline _pipeline;

  public SeedWorldsTaskHandler(ILogger<SeedWorldsTaskHandler> logger, IRequestPipeline pipeline)
  {
    _logger = logger;
    _pipeline = pipeline;
  }

  public async Task Handle(SeedWorldsTask _, CancellationToken cancellationToken)
  {
    string json = await File.ReadAllTextAsync("Backend/worlds.json", Encoding.UTF8, cancellationToken);
    IEnumerable<WorldPayload>? payloads = JsonSerializer.Deserialize<IEnumerable<WorldPayload>>(json, _serializerOptions);
    if (payloads != null)
    {
      foreach (WorldPayload payload in payloads)
      {
        CreateOrReplaceWorldCommand command = new(payload.Id, payload, Version: null);
        CreateOrReplaceWorldResult result = await _pipeline.ExecuteAsync(command, cancellationToken);
        WorldModel world = result.World ?? throw new InvalidOperationException("The world model should not be null.");
        string status = result.Created ? "created" : "updated";
        _logger.LogInformation("The world '{Name}' has been {Status} (Id={Id}).", world.Name, status, world.Id);
      }
    }
  }
}
