using CsvHelper;
using CsvHelper.Configuration.Attributes;
using MediatR;
using SkillCraft.Application.Aspects.Commands;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Aspects;
using System.Globalization;
using Attribute = SkillCraft.Contracts.Attribute;

namespace SkillCraft.Tools.ETL.Commands;

internal record ImportAspects : INotification;

internal class ImportAspectsHandler : INotificationHandler<ImportAspects>
{
  private readonly IApiClient _client;

  public ImportAspectsHandler(IApiClient client)
  {
    _client = client;
  }

  public async Task Handle(ImportAspects _, CancellationToken cancellationToken)
  {
    IReadOnlyCollection<Aspect> aspects = Extract();
    IReadOnlyCollection<SaveAspectCommand> commands = Transform(aspects);
    await LoadAsync(commands, cancellationToken);
  }

  private static IReadOnlyCollection<Aspect> Extract()
  {
    StreamReader reader = new("data/aspects.csv");
    CsvReader csv = new(reader, CultureInfo.InvariantCulture);
    IEnumerable<Aspect> records = csv.GetRecords<Aspect>();
    return records.ToArray().AsReadOnly();
  }

  private static IReadOnlyCollection<SaveAspectCommand> Transform(IEnumerable<Aspect> aspects) => aspects.Select(aspect =>
  {
    SaveAspectPayload payload = new(aspect.Name)
    {
      Description = aspect.Description,
      Attributes = new AttributeSelectionModel
      {
        Mandatory1 = aspect.MandatoryAttribute1,
        Mandatory2 = aspect.MandatoryAttribute2,
        Optional1 = aspect.OptionalAttribute1,
        Optional2 = aspect.OptionalAttribute2
      },
      Skills = new SkillsModel
      {
        Discounted1 = aspect.DiscountedSkill1,
        Discounted2 = aspect.DiscountedSkill2
      }
    };
    return new SaveAspectCommand(aspect.Id, payload, Version: null);
  }).ToArray().AsReadOnly();

  private async Task LoadAsync(IEnumerable<SaveAspectCommand> commands, CancellationToken cancellationToken)
  {
    foreach (SaveAspectCommand command in commands)
    {
      await _client.SaveAspectAsync(command, cancellationToken);
    }
  }
}

internal record Aspect
{
  [Name("id")]
  public Guid Id { get; set; }

  [Name("name")]
  public string Name { get; set; } = string.Empty;

  [Name("description")]
  public string? Description { get; set; }

  [Name("attributes.mandatory1")]
  public Attribute? MandatoryAttribute1 { get; set; }

  [Name("attributes.mandatory2")]
  public Attribute? MandatoryAttribute2 { get; set; }

  [Name("attributes.optional1")]
  public Attribute? OptionalAttribute1 { get; set; }

  [Name("attributes.optional2")]
  public Attribute? OptionalAttribute2 { get; set; }

  [Name("skills.discounted1")]
  public Skill? DiscountedSkill1 { get; set; }

  [Name("skills.discounted2")]
  public Skill? DiscountedSkill2 { get; set; }
}
