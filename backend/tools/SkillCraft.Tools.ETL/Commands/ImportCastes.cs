using CsvHelper;
using CsvHelper.Configuration.Attributes;
using MediatR;
using SkillCraft.Application.Castes.Commands;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Castes;

namespace SkillCraft.Tools.ETL.Commands;

internal record ImportCastes : INotification;

internal class ImportCastesHandler : INotificationHandler<ImportCastes>
{
  private readonly IApiClient _client;

  public ImportCastesHandler(IApiClient client)
  {
    _client = client;
  }

  public async Task Handle(ImportCastes _, CancellationToken cancellationToken)
  {
    IReadOnlyCollection<Caste> castes = Extract();
    IReadOnlyCollection<CreateOrReplaceCasteCommand> commands = Transform(castes);
    await LoadAsync(commands, cancellationToken);
  }

  private static IReadOnlyCollection<Caste> Extract()
  {
    StreamReader reader = new("data/castes.csv");
    CsvReader csv = new(reader, CultureInfo.InvariantCulture);
    IEnumerable<Caste> records = csv.GetRecords<Caste>();
    return records.ToArray().AsReadOnly();
  }

  private static IReadOnlyCollection<CreateOrReplaceCasteCommand> Transform(IEnumerable<Caste> castes) => castes.Select(caste =>
  {
    CreateOrReplaceCastePayload payload = new(caste.Name)
    {
      Description = caste.Description,
      Skill = caste.Skill,
      WealthRoll = caste.WealthRoll
    };
    if (!string.IsNullOrWhiteSpace(caste.Trait1Name))
    {
      payload.Traits.Add(new TraitPayload(caste.Trait1Name)
      {
        Id = caste.Trait1Id,
        Description = caste.Trait1Description
      });
    }
    if (!string.IsNullOrWhiteSpace(caste.Trait2Name))
    {
      payload.Traits.Add(new TraitPayload(caste.Trait2Name)
      {
        Id = caste.Trait2Id,
        Description = caste.Trait2Description
      });
    }
    return new CreateOrReplaceCasteCommand(caste.Id, payload, Version: null);
  }).ToArray().AsReadOnly();

  private async Task LoadAsync(IEnumerable<CreateOrReplaceCasteCommand> commands, CancellationToken cancellationToken)
  {
    foreach (CreateOrReplaceCasteCommand command in commands)
    {
      await _client.CreateOrReplaceCasteAsync(command, cancellationToken);
    }
  }
}

internal record Caste
{
  [Name("id")]
  public Guid Id { get; set; }

  [Name("name")]
  public string Name { get; set; } = string.Empty;

  [Name("description")]
  public string? Description { get; set; }

  [Name("skill")]
  public Skill? Skill { get; set; }

  [Name("wealthRoll")]
  public string? WealthRoll { get; set; }

  [Name("traits[0].id")]
  public Guid? Trait1Id { get; set; }

  [Name("traits[0].name")]
  public string? Trait1Name { get; set; }

  [Name("traits[0].description")]
  public string? Trait1Description { get; set; }

  [Name("traits[1].id")]
  public Guid? Trait2Id { get; set; }

  [Name("traits[1].name")]
  public string? Trait2Name { get; set; }

  [Name("traits[1].description")]
  public string? Trait2Description { get; set; }
}
