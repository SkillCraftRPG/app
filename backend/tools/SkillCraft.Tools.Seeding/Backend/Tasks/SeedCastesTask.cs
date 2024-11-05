using MediatR;
using SkillCraft.Application;
using SkillCraft.Application.Castes.Commands;
using SkillCraft.Contracts.Castes;

namespace SkillCraft.Tools.Seeding.Backend.Tasks;

internal class SeedCastesTask : SeedingTask
{
  public override string? Description => "Seeds the character castes.";
}

internal class SeedCastesTaskHandler : INotificationHandler<SeedCastesTask>
{
  private static readonly JsonSerializerOptions _serializerOptions = new();
  static SeedCastesTaskHandler()
  {
    _serializerOptions.Converters.Add(new JsonStringEnumConverter());
  }

  private readonly ILogger<SeedCastesTaskHandler> _logger;
  private readonly IRequestPipeline _pipeline;

  public SeedCastesTaskHandler(ILogger<SeedCastesTaskHandler> logger, IRequestPipeline pipeline)
  {
    _logger = logger;
    _pipeline = pipeline;
  }

  public async Task Handle(SeedCastesTask _, CancellationToken cancellationToken)
  {
    string json = await File.ReadAllTextAsync("Backend/castes.json", Encoding.UTF8, cancellationToken);
    IEnumerable<CastePayload>? payloads = JsonSerializer.Deserialize<IEnumerable<CastePayload>>(json, _serializerOptions);
    if (payloads != null)
    {
      foreach (CastePayload payload in payloads)
      {
        CreateOrReplaceCasteCommand command = new(payload.Id, payload, Version: null);
        CreateOrReplaceCasteResult result = await _pipeline.ExecuteAsync(command, cancellationToken);
        CasteModel caste = result.Caste ?? throw new InvalidOperationException("The caste model should not be null.");
        string status = result.Created ? "created" : "updated";
        _logger.LogInformation("The caste '{Name}' has been {Status} (Id={Id}).", caste.Name, status, caste.Id);
      }
    }
  }
}
