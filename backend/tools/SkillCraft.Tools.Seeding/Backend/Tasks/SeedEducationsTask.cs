using MediatR;
using SkillCraft.Application;
using SkillCraft.Application.Educations.Commands;
using SkillCraft.Contracts.Educations;

namespace SkillCraft.Tools.Seeding.Backend.Tasks;

internal class SeedEducationsTask : SeedingTask
{
  public override string? Description => "Seeds the character educations.";
}

internal class SeedEducationsTaskHandler : INotificationHandler<SeedEducationsTask>
{
  private static readonly JsonSerializerOptions _serializerOptions = new();
  static SeedEducationsTaskHandler()
  {
    _serializerOptions.Converters.Add(new JsonStringEnumConverter());
  }

  private readonly ILogger<SeedEducationsTaskHandler> _logger;
  private readonly IRequestPipeline _pipeline;

  public SeedEducationsTaskHandler(ILogger<SeedEducationsTaskHandler> logger, IRequestPipeline pipeline)
  {
    _logger = logger;
    _pipeline = pipeline;
  }

  public async Task Handle(SeedEducationsTask _, CancellationToken cancellationToken)
  {
    string json = await File.ReadAllTextAsync("Backend/educations.json", Encoding.UTF8, cancellationToken);
    IEnumerable<EducationPayload>? payloads = JsonSerializer.Deserialize<IEnumerable<EducationPayload>>(json, _serializerOptions);
    if (payloads != null)
    {
      foreach (EducationPayload payload in payloads)
      {
        CreateOrReplaceEducationCommand command = new(payload.Id, payload, Version: null);
        CreateOrReplaceEducationResult result = await _pipeline.ExecuteAsync(command, cancellationToken);
        EducationModel education = result.Education ?? throw new InvalidOperationException("The education model should not be null.");
        string status = result.Created ? "created" : "updated";
        _logger.LogInformation("The education '{Name}' has been {Status} (Id={Id}).", education.Name, status, education.Id);
      }
    }
  }
}
