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
    IReadOnlyCollection<CreateOrReplacePartyCommand> commands = Transform(parties);
    await LoadAsync(commands, cancellationToken);
  }

  private static IReadOnlyCollection<Party> Extract()
  {
    StreamReader reader = new("data/parties.csv");
    CsvReader csv = new(reader, CultureInfo.InvariantCulture);
    IEnumerable<Party> records = csv.GetRecords<Party>();
    return records.ToArray().AsReadOnly();
  }

  private static IReadOnlyCollection<CreateOrReplacePartyCommand> Transform(IEnumerable<Party> parties) => parties.Select(party =>
  {
    CreateOrReplacePartyPayload payload = new(party.Name)
    {
      Description = party.Description
    };
    return new CreateOrReplacePartyCommand(party.Id, payload, Version: null);
  }).ToArray().AsReadOnly();

  private async Task LoadAsync(IEnumerable<CreateOrReplacePartyCommand> commands, CancellationToken cancellationToken)
  {
    foreach (CreateOrReplacePartyCommand command in commands)
    {
      await _client.CreateOrReplacePartyAsync(command, cancellationToken);
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
