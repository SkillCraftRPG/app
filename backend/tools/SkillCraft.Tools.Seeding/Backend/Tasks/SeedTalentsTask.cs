using MediatR;
using SkillCraft.Application;
using SkillCraft.Application.Talents.Commands;
using SkillCraft.Contracts.Talents;

namespace SkillCraft.Tools.Seeding.Backend.Tasks;

internal class SeedTalentsTask : SeedingTask
{
  public override string? Description => "Seeds the character talents.";
}

internal class SeedTalentsTaskHandler : INotificationHandler<SeedTalentsTask>
{
  private static readonly JsonSerializerOptions _serializerOptions = new();
  static SeedTalentsTaskHandler()
  {
    _serializerOptions.Converters.Add(new JsonStringEnumConverter());
  }

  private readonly ILogger<SeedTalentsTaskHandler> _logger;
  private readonly IRequestPipeline _pipeline;

  public SeedTalentsTaskHandler(ILogger<SeedTalentsTaskHandler> logger, IRequestPipeline pipeline)
  {
    _logger = logger;
    _pipeline = pipeline;
  }

  public async Task Handle(SeedTalentsTask _, CancellationToken cancellationToken)
  {
    string json = await File.ReadAllTextAsync("Backend/talents.json", Encoding.UTF8, cancellationToken);
    IEnumerable<TalentPayload>? payloads = JsonSerializer.Deserialize<IEnumerable<TalentPayload>>(json, _serializerOptions);
    if (payloads != null)
    {
      foreach (TalentPayload payload in payloads)
      {
        CreateOrReplaceTalentCommand command = new(payload.Id, payload, Version: null);
        CreateOrReplaceTalentResult result = await _pipeline.ExecuteAsync(command, cancellationToken);
        TalentModel talent = result.Talent ?? throw new InvalidOperationException("The talent model should not be null.");
        string status = result.Created ? "created" : "updated";
        _logger.LogInformation("The talent '{Name}' has been {Status} (Id={Id}).", talent.Name, status, talent.Id);
      }
    }
  }
}
