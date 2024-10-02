using CsvHelper;
using CsvHelper.Configuration.Attributes;
using MediatR;
using SkillCraft.Application.Parties.Commands;
using SkillCraft.Contracts.Parties;

namespace SkillCraft.Tools.ETL.Commands;

internal record ImportParties : INotification;

internal class ImportPartiesHandler : INotificationHandler<ImportParties>
{
  private readonly IApiClient _client;

  public ImportPartiesHandler(IApiClient client)
  {
    _client = client;
  }

  public async Task Handle(ImportParties _, CancellationToken cancellationToken)
  {
    IReadOnlyCollection<Party> parties = Extract();
    IReadOnlyCollection<SavePartyCommand> commands = Transform(parties);
    await LoadAsync(commands, cancellationToken);
  }

  private static IReadOnlyCollection<Party> Extract()
  {
    StreamReader reader = new("data/parties.csv");
    CsvReader csv = new(reader, CultureInfo.InvariantCulture);
    IEnumerable<Party> records = csv.GetRecords<Party>();
    return records.ToArray().AsReadOnly();
  }

  private static IReadOnlyCollection<SavePartyCommand> Transform(IEnumerable<Party> parties) => parties.Select(party =>
  {
    SavePartyPayload payload = new(party.Name)
    {
      Description = party.Description
    };
    return new SavePartyCommand(party.Id, payload, Version: null);
  }).ToArray().AsReadOnly();

  private async Task LoadAsync(IEnumerable<SavePartyCommand> commands, CancellationToken cancellationToken)
  {
    foreach (SavePartyCommand command in commands)
    {
      await _client.SavePartyAsync(command, cancellationToken);
    }
  }
}

internal record Party
{
  [Name("id")]
  public Guid Id { get; set; }

  [Name("name")]
  public string Name { get; set; } = string.Empty;

  [Name("description")]
  public string? Description { get; set; }
}
