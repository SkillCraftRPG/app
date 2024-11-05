using MediatR;
using SkillCraft.Application;
using SkillCraft.Application.Languages.Commands;
using SkillCraft.Contracts.Languages;

namespace SkillCraft.Tools.Seeding.Backend.Tasks;

internal class SeedLanguagesTask : SeedingTask
{
  public override string? Description => "Seeds the character languages.";
}

internal class SeedLanguagesTaskHandler : INotificationHandler<SeedLanguagesTask>
{
  private static readonly JsonSerializerOptions _serializerOptions = new();
  static SeedLanguagesTaskHandler()
  {
    _serializerOptions.Converters.Add(new JsonStringEnumConverter());
  }

  private readonly ILogger<SeedLanguagesTaskHandler> _logger;
  private readonly IRequestPipeline _pipeline;

  public SeedLanguagesTaskHandler(ILogger<SeedLanguagesTaskHandler> logger, IRequestPipeline pipeline)
  {
    _logger = logger;
    _pipeline = pipeline;
  }

  public async Task Handle(SeedLanguagesTask _, CancellationToken cancellationToken)
  {
    string json = await File.ReadAllTextAsync("Backend/languages.json", Encoding.UTF8, cancellationToken);
    IEnumerable<LanguagePayload>? payloads = JsonSerializer.Deserialize<IEnumerable<LanguagePayload>>(json, _serializerOptions);
    if (payloads != null)
    {
      foreach (LanguagePayload payload in payloads)
      {
        CreateOrReplaceLanguageCommand command = new(payload.Id, payload, Version: null);
        CreateOrReplaceLanguageResult result = await _pipeline.ExecuteAsync(command, cancellationToken);
        LanguageModel language = result.Language ?? throw new InvalidOperationException("The language model should not be null.");
        string status = result.Created ? "created" : "updated";
        _logger.LogInformation("The language '{Name}' has been {Status} (Id={Id}).", language.Name, status, language.Id);
      }
    }
  }
}
