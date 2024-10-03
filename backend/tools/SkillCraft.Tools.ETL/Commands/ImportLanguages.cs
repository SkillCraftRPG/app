using CsvHelper;
using CsvHelper.Configuration.Attributes;
using MediatR;
using SkillCraft.Application.Languages.Commands;
using SkillCraft.Contracts.Languages;

namespace SkillCraft.Tools.ETL.Commands;

internal record ImportLanguages : INotification;

internal class ImportLanguagesHandler : INotificationHandler<ImportLanguages>
{
  private readonly IApiClient _client;

  public ImportLanguagesHandler(IApiClient client)
  {
    _client = client;
  }

  public async Task Handle(ImportLanguages _, CancellationToken cancellationToken)
  {
    IReadOnlyCollection<Language> languages = Extract();
    IReadOnlyCollection<CreateOrReplaceLanguageCommand> commands = Transform(languages);
    await LoadAsync(commands, cancellationToken);
  }

  private static IReadOnlyCollection<Language> Extract()
  {
    StreamReader reader = new("data/languages.csv");
    CsvReader csv = new(reader, CultureInfo.InvariantCulture);
    IEnumerable<Language> records = csv.GetRecords<Language>();
    return records.ToArray().AsReadOnly();
  }

  private static IReadOnlyCollection<CreateOrReplaceLanguageCommand> Transform(IEnumerable<Language> languages) => languages.Select(language =>
  {
    CreateOrReplaceLanguagePayload payload = new(language.Name)
    {
      Description = language.Description,
      Script = language.Script,
      TypicalSpeakers = language.TypicalSpeakers
    };
    return new CreateOrReplaceLanguageCommand(language.Id, payload, Version: null);
  }).ToArray().AsReadOnly();

  private async Task LoadAsync(IEnumerable<CreateOrReplaceLanguageCommand> commands, CancellationToken cancellationToken)
  {
    foreach (CreateOrReplaceLanguageCommand command in commands)
    {
      await _client.CreateOrReplaceLanguageAsync(command, cancellationToken);
    }
  }
}

internal record Language
{
  [Name("id")]
  public Guid Id { get; set; }

  [Name("name")]
  public string Name { get; set; } = string.Empty;

  [Name("description")]
  public string? Description { get; set; }

  [Name("script")]
  public string? Script { get; set; }

  [Name("typicalSpeakers")]
  public string? TypicalSpeakers { get; set; }
}
