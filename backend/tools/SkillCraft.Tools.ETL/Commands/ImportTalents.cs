using CsvHelper;
using CsvHelper.Configuration.Attributes;
using MediatR;
using SkillCraft.Application.Talents.Commands;
using SkillCraft.Contracts.Talents;

namespace SkillCraft.Tools.ETL.Commands;

internal record ImportTalents : INotification;

internal class ImportTalentsHandler : INotificationHandler<ImportTalents>
{
  private readonly IApiClient _client;

  public ImportTalentsHandler(IApiClient client)
  {
    _client = client;
  }

  public async Task Handle(ImportTalents _, CancellationToken cancellationToken)
  {
    IReadOnlyCollection<Talent> talents = Extract();
    IReadOnlyCollection<CreateOrReplaceTalentCommand> commands = Transform(talents);
    await LoadAsync(commands, cancellationToken);
  }

  private static IReadOnlyCollection<Talent> Extract()
  {
    StreamReader reader = new("data/talents.csv");
    CsvReader csv = new(reader, CultureInfo.InvariantCulture);
    IEnumerable<Talent> records = csv.GetRecords<Talent>();
    return records.ToArray().AsReadOnly();
  }

  private static IReadOnlyCollection<CreateOrReplaceTalentCommand> Transform(IEnumerable<Talent> talents) => talents.Select(talent =>
  {
    CreateOrReplaceTalentPayload payload = new(talent.Name)
    {
      Tier = talent.Tier,
      Description = talent.Description,
      AllowMultiplePurchases = talent.AllowMultiplePurchases?.Trim().Equals(bool.TrueString, StringComparison.InvariantCulture) == true,
      RequiredTalentId = talent.RequiredTalentId
    };
    return new CreateOrReplaceTalentCommand(talent.Id, payload, Version: null);
  }).ToArray().AsReadOnly();

  private async Task LoadAsync(IEnumerable<CreateOrReplaceTalentCommand> commands, CancellationToken cancellationToken)
  {
    foreach (CreateOrReplaceTalentCommand command in commands)
    {
      await _client.CreateOrReplaceTalentAsync(command, cancellationToken);
    }
  }
}

internal record Talent
{
  [Name("id")]
  public Guid Id { get; set; }

  [Name("tier")]
  public int Tier { get; set; }

  [Name("name")]
  public string Name { get; set; } = string.Empty;

  [Name("description")]
  public string? Description { get; set; }

  [Name("allowMultiplePurchases")]
  public string? AllowMultiplePurchases { get; set; }

  [Name("requiredTalentId")]
  public Guid? RequiredTalentId { get; set; }
}
