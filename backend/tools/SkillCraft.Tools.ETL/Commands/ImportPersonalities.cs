using CsvHelper;
using CsvHelper.Configuration.Attributes;
using MediatR;
using SkillCraft.Application.Personalities.Commands;
using SkillCraft.Contracts.Personalities;
using Attribute = SkillCraft.Contracts.Attribute;

namespace SkillCraft.Tools.ETL.Commands;

internal record ImportPersonalities : INotification;

internal class ImportPersonalitiesHandler : INotificationHandler<ImportPersonalities>
{
  private readonly IApiClient _client;

  public ImportPersonalitiesHandler(IApiClient client)
  {
    _client = client;
  }

  public async Task Handle(ImportPersonalities _, CancellationToken cancellationToken)
  {
    IReadOnlyCollection<Personality> personalities = Extract();
    IReadOnlyCollection<CreateOrReplacePersonalityCommand> commands = Transform(personalities);
    await LoadAsync(commands, cancellationToken);
  }

  private static IReadOnlyCollection<Personality> Extract()
  {
    StreamReader reader = new("data/personalities.csv");
    CsvReader csv = new(reader, CultureInfo.InvariantCulture);
    IEnumerable<Personality> records = csv.GetRecords<Personality>();
    return records.ToArray().AsReadOnly();
  }

  private static IReadOnlyCollection<CreateOrReplacePersonalityCommand> Transform(IEnumerable<Personality> personalities) => personalities.Select(personality =>
  {
    CreateOrReplacePersonalityPayload payload = new(personality.Name)
    {
      Description = personality.Description,
      Attribute = personality.Attribute,
      GiftId = personality.GiftId
    };
    return new CreateOrReplacePersonalityCommand(personality.Id, payload, Version: null);
  }).ToArray().AsReadOnly();

  private async Task LoadAsync(IEnumerable<CreateOrReplacePersonalityCommand> commands, CancellationToken cancellationToken)
  {
    foreach (CreateOrReplacePersonalityCommand command in commands)
    {
      await _client.SavePersonalityAsync(command, cancellationToken);
    }
  }
}

internal record Personality
{
  [Name("id")]
  public Guid Id { get; set; }

  [Name("name")]
  public string Name { get; set; } = string.Empty;

  [Name("description")]
  public string? Description { get; set; }

  [Name("attribute")]
  public Attribute Attribute { get; set; }

  [Name("giftId")]
  public Guid GiftId { get; set; }
}
