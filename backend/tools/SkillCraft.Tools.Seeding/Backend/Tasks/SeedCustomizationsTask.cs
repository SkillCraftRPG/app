using MediatR;
using SkillCraft.Application;
using SkillCraft.Application.Customizations.Commands;
using SkillCraft.Contracts.Customizations;

namespace SkillCraft.Tools.Seeding.Backend.Tasks;

internal class SeedCustomizationsTask : SeedingTask
{
  public override string? Description => "Seeds the character customizations.";
}

internal class SeedCustomizationsTaskHandler : INotificationHandler<SeedCustomizationsTask>
{
  private static readonly JsonSerializerOptions _serializerOptions = new();
  static SeedCustomizationsTaskHandler()
  {
    _serializerOptions.Converters.Add(new JsonStringEnumConverter());
  }

  private readonly ILogger<SeedCustomizationsTaskHandler> _logger;
  private readonly IRequestPipeline _pipeline;

  public SeedCustomizationsTaskHandler(ILogger<SeedCustomizationsTaskHandler> logger, IRequestPipeline pipeline)
  {
    _logger = logger;
    _pipeline = pipeline;
  }

  public async Task Handle(SeedCustomizationsTask _, CancellationToken cancellationToken)
  {
    string json = await File.ReadAllTextAsync("Backend/customizations.json", Encoding.UTF8, cancellationToken);
    IEnumerable<CustomizationPayload>? payloads = JsonSerializer.Deserialize<IEnumerable<CustomizationPayload>>(json, _serializerOptions);
    if (payloads != null)
    {
      foreach (CustomizationPayload payload in payloads)
      {
        CreateOrReplaceCustomizationCommand command = new(payload.Id, payload, Version: null);
        CreateOrReplaceCustomizationResult result = await _pipeline.ExecuteAsync(command, cancellationToken);
        CustomizationModel customization = result.Customization ?? throw new InvalidOperationException("The customization model should not be null.");
        string status = result.Created ? "created" : "updated";
        _logger.LogInformation("The customization '{Name}' has been {Status} (Id={Id}).", customization.Name, status, customization.Id);
      }
    }
  }
}
