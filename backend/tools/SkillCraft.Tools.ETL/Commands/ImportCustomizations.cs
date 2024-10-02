using CsvHelper;
using CsvHelper.Configuration.Attributes;
using MediatR;
using SkillCraft.Application.Customizations.Commands;
using SkillCraft.Contracts.Customizations;

namespace SkillCraft.Tools.ETL.Commands;

internal record ImportCustomizations : INotification;

internal class ImportCustomizationsHandler : INotificationHandler<ImportCustomizations>
{
  private readonly IApiClient _client;

  public ImportCustomizationsHandler(IApiClient client)
  {
    _client = client;
  }

  public async Task Handle(ImportCustomizations _, CancellationToken cancellationToken)
  {
    IReadOnlyCollection<Customization> customizations = Extract();
    IReadOnlyCollection<SaveCustomizationCommand> commands = Transform(customizations);
    await LoadAsync(commands, cancellationToken);
  }

  private static IReadOnlyCollection<Customization> Extract()
  {
    StreamReader reader = new("data/customizations.csv");
    CsvReader csv = new(reader, CultureInfo.InvariantCulture);
    IEnumerable<Customization> records = csv.GetRecords<Customization>();
    return records.ToArray().AsReadOnly();
  }

  private static IReadOnlyCollection<SaveCustomizationCommand> Transform(IEnumerable<Customization> customizations) => customizations.Select(customization =>
  {
    SaveCustomizationPayload payload = new(customization.Name)
    {
      Type = customization.Type,
      Description = customization.Description
    };
    return new SaveCustomizationCommand(customization.Id, payload, Version: null);
  }).ToArray().AsReadOnly();

  private async Task LoadAsync(IEnumerable<SaveCustomizationCommand> commands, CancellationToken cancellationToken)
  {
    foreach (SaveCustomizationCommand command in commands)
    {
      await _client.SaveCustomizationAsync(command, cancellationToken);
    }
  }
}

internal record Customization
{
  [Name("id")]
  public Guid Id { get; set; }

  [Name("type")]
  public CustomizationType Type { get; set; }

  [Name("name")]
  public string Name { get; set; } = string.Empty;

  [Name("description")]
  public string? Description { get; set; }
}
