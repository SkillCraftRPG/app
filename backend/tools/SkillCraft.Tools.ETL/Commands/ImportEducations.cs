using CsvHelper;
using CsvHelper.Configuration.Attributes;
using MediatR;
using SkillCraft.Application.Educations.Commands;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Educations;
using System.Globalization;

namespace SkillCraft.Tools.ETL.Commands;

internal record ImportEducations : INotification;

internal class ImportEducationsHandler : INotificationHandler<ImportEducations>
{
  private readonly IApiClient _client;

  public ImportEducationsHandler(IApiClient client)
  {
    _client = client;
  }

  public async Task Handle(ImportEducations _, CancellationToken cancellationToken)
  {
    IReadOnlyCollection<Education> educations = Extract();
    IReadOnlyCollection<SaveEducationCommand> commands = Transform(educations);
    await LoadAsync(commands, cancellationToken);
  }

  private static IReadOnlyCollection<Education> Extract()
  {
    StreamReader reader = new("data/educations.csv");
    CsvReader csv = new(reader, CultureInfo.InvariantCulture);
    IEnumerable<Education> records = csv.GetRecords<Education>();
    return records.ToArray().AsReadOnly();
  }

  private static IReadOnlyCollection<SaveEducationCommand> Transform(IEnumerable<Education> educations) => educations.Select(education =>
  {
    SaveEducationPayload payload = new(education.Name)
    {
      Description = education.Description,
      Skill = education.Skill,
      WealthMultiplier = education.WealthMultiplier
    };
    return new SaveEducationCommand(education.Id, payload, Version: null);
  }).ToArray().AsReadOnly();

  private async Task LoadAsync(IEnumerable<SaveEducationCommand> commands, CancellationToken cancellationToken)
  {
    foreach (SaveEducationCommand command in commands)
    {
      await _client.SaveEducationAsync(command, cancellationToken);
    }
  }
}

internal record Education
{
  [Name("id")]
  public Guid Id { get; set; }

  [Name("name")]
  public string Name { get; set; } = string.Empty;

  [Name("description")]
  public string? Description { get; set; }

  [Name("skill")]
  public Skill? Skill { get; set; }

  [Name("wealthMultiplier")]
  public double? WealthMultiplier { get; set; }
}
