using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Search;
using Logitar.Portal.Contracts.Templates;
using MediatR;

namespace SkillCraft.Tools.Seeding.Portal.Tasks;

internal class SeedTemplatesTaskHandler : INotificationHandler<SeedTemplatesTask>
{
  private static readonly Dictionary<string, string> _contentTypes = new()
  {
    [".html"] = MediaTypeNames.Text.Html,
    [".txt"] = MediaTypeNames.Text.Plain
  };

  private readonly ILogger<SeedTemplatesTaskHandler> _logger;
  private readonly ITemplateClient _templates;

  public SeedTemplatesTaskHandler(ILogger<SeedTemplatesTaskHandler> logger, ITemplateClient templates)
  {
    _logger = logger;
    _templates = templates;
  }

  public async Task Handle(SeedTemplatesTask _, CancellationToken cancellationToken)
  {
    RequestContext context = new(cancellationToken);

    string json = await File.ReadAllTextAsync("Portal/templates.json", Encoding.UTF8, cancellationToken);
    IEnumerable<TemplateSummary>? summaries = JsonSerializer.Deserialize<IEnumerable<TemplateSummary>>(json);
    if (summaries != null)
    {
      SearchResults<Template> results = await _templates.SearchAsync(new SearchTemplatesPayload(), context);
      Dictionary<string, Template> templates = new(capacity: results.Items.Count);
      foreach (Template template in results.Items)
      {
        templates[template.UniqueKey] = template;
      }

      Dictionary<string, Content> contents = await LoadContentsAsync(cancellationToken);

      foreach (TemplateSummary summary in summaries)
      {
        string status = "created";
        string subject = $"{summary.UniqueKey}_Subject";
        Content content = contents[summary.UniqueKey];
        if (templates.TryGetValue(summary.UniqueKey, out Template? template))
        {
          status = "updated";
        }
        else
        {
          CreateTemplatePayload payload = new(summary.UniqueKey, subject, content);
          template = await _templates.CreateAsync(payload, context);
          templates[summary.UniqueKey] = template;
        }

        ReplaceTemplatePayload replace = new(summary.UniqueKey, subject, content)
        {
          DisplayName = summary.DisplayName,
          Description = summary.Description
        };
        template = await _templates.ReplaceAsync(template.Id, replace, template.Version)
          ?? throw new InvalidOperationException($"The template 'Id={template.Id}' replace result should not be null.");

        _logger.LogInformation("The template '{UniqueKey}' has been {Status} (Id={Id}).", template.UniqueKey, status, template.Id);
      }
    }
  }

  private static async Task<Dictionary<string, Content>> LoadContentsAsync(CancellationToken cancellationToken)
  {
    string[] files = Directory.GetFiles("Portal/Templates");
    Dictionary<string, Content> contents = new(capacity: files.Length);

    foreach (string path in files)
    {
      string name = Path.GetFileNameWithoutExtension(path);
      string extension = Path.GetExtension(path);

      string type = _contentTypes[extension];
      string text = await File.ReadAllTextAsync(path, Encoding.UTF8, cancellationToken);

      contents[name] = new(type, text);
    }

    return contents;
  }
}
