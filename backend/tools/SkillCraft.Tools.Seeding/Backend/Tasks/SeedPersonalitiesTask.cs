using MediatR;
using SkillCraft.Application;
using SkillCraft.Application.Personalities.Commands;
using SkillCraft.Contracts.Personalities;

namespace SkillCraft.Tools.Seeding.Backend.Tasks;

internal class SeedPersonalitiesTask : SeedingTask
{
  public override string? Description => "Seeds the character personalities.";
}

internal class SeedPersonalitiesTaskHandler : INotificationHandler<SeedPersonalitiesTask>
{
  private static readonly JsonSerializerOptions _serializerOptions = new();
  static SeedPersonalitiesTaskHandler()
  {
    _serializerOptions.Converters.Add(new JsonStringEnumConverter());
  }

  private readonly ILogger<SeedPersonalitiesTaskHandler> _logger;
  private readonly IRequestPipeline _pipeline;

  public SeedPersonalitiesTaskHandler(ILogger<SeedPersonalitiesTaskHandler> logger, IRequestPipeline pipeline)
  {
    _logger = logger;
    _pipeline = pipeline;
  }

  public async Task Handle(SeedPersonalitiesTask _, CancellationToken cancellationToken)
  {
    string json = await File.ReadAllTextAsync("Backend/personalities.json", Encoding.UTF8, cancellationToken);
    IEnumerable<PersonalityPayload>? payloads = JsonSerializer.Deserialize<IEnumerable<PersonalityPayload>>(json, _serializerOptions);
    if (payloads != null)
    {
      foreach (PersonalityPayload payload in payloads)
      {
        CreateOrReplacePersonalityCommand command = new(payload.Id, payload, Version: null);
        CreateOrReplacePersonalityResult result = await _pipeline.ExecuteAsync(command, cancellationToken);
        PersonalityModel personality = result.Personality ?? throw new InvalidOperationException("The personality model should not be null.");
        string status = result.Created ? "created" : "updated";
        _logger.LogInformation("The personality '{Name}' has been {Status} (Id={Id}).", personality.Name, status, personality.Id);
      }
    }
  }
}
