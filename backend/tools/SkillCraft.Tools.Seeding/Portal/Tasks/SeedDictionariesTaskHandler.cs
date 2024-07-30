using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Dictionaries;
using Logitar.Portal.Contracts.Search;
using MediatR;

namespace SkillCraft.Tools.Seeding.Portal.Tasks;

internal class SeedDictionariesTaskHandler : INotificationHandler<SeedDictionariesTask>
{
  private readonly IDictionaryClient _dictionaries;
  private readonly ILogger<SeedDictionariesTaskHandler> _logger;

  public SeedDictionariesTaskHandler(IDictionaryClient dictionaries, ILogger<SeedDictionariesTaskHandler> logger)
  {
    _dictionaries = dictionaries;
    _logger = logger;
  }

  public async Task Handle(SeedDictionariesTask _, CancellationToken cancellationToken)
  {
    RequestContext context = new(cancellationToken);

    string[] files = Directory.GetFiles("Portal/Dictionaries");
    if (files.Length > 0)
    {
      SearchResults<Dictionary> results = await _dictionaries.SearchAsync(new SearchDictionariesPayload(), context);
      Dictionary<string, Dictionary> dictionaries = new(capacity: results.Items.Count);
      foreach (Dictionary dictionary in results.Items)
      {
        dictionaries[dictionary.Locale.Code] = dictionary;
      }

      foreach (string path in files)
      {
        string status = "created";
        string locale = Path.GetFileNameWithoutExtension(path);
        if (dictionaries.TryGetValue(locale, out Dictionary? dictionary))
        {
          status = "updated";
        }
        else
        {
          CreateDictionaryPayload payload = new(locale);
          dictionary = await _dictionaries.CreateAsync(payload, context);
          dictionaries[locale] = dictionary;
        }

        string json = await File.ReadAllTextAsync(path, Encoding.UTF8, cancellationToken);
        Dictionary<string, string>? entries = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
        if (entries != null)
        {
          ReplaceDictionaryPayload replace = new(locale);
          foreach (KeyValuePair<string, string> entry in entries)
          {
            replace.Entries.Add(new DictionaryEntry(entry));
          }
          dictionary = await _dictionaries.ReplaceAsync(dictionary.Id, replace, dictionary.Version, context)
            ?? throw new InvalidOperationException($"The dictionary 'Id={dictionary.Id}' replace result should not be null.");
        }

        _logger.LogInformation("The dictionary '{Locale}' has been {Status} (Id={Id}).", dictionary.Locale.Code, status, dictionary.Id);
      }
    }
  }
}
