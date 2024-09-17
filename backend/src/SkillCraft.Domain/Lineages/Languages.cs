using Logitar;
using SkillCraft.Domain.Languages;
using System.Collections.Immutable;

namespace SkillCraft.Domain.Lineages;

public record Languages
{
  public const int MaximumLength = byte.MaxValue;

  [JsonIgnore]
  private readonly ImmutableHashSet<LanguageId> _ids;

  public IEnumerable<LanguageId> Ids => _ids;
  public int Extra { get; }
  public string? Text { get; }

  public Languages()
    : this(ids: [], extra: 0, text: null)
  {
  }

  public Languages(IEnumerable<Language> languages, int extra, string? text)
    : this(languages.Select(language => language.Id), extra, text)
  {
  }

  [JsonConstructor]
  public Languages(IEnumerable<LanguageId> ids, int extra, string? text)
  {
    _ids = ImmutableHashSet.Create(ids.ToArray());
    Extra = extra;
    Text = text?.CleanTrim();
  }

  public bool Has(LanguageId id) => _ids.Contains(id);
}
