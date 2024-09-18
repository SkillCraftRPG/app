using FluentValidation;
using Logitar;
using SkillCraft.Domain.Languages;
using SkillCraft.Domain.Lineages.Validators;

namespace SkillCraft.Domain.Lineages;

public record Languages
{
  public const int MaximumLength = byte.MaxValue;

  public IReadOnlyCollection<LanguageId> Ids { get; }
  public int Extra { get; }
  public string? Text { get; }

  public int Size => (Ids.Count * 4) + 4 + (Text?.Length ?? 0);

  public Languages()
    : this(ids: [], extra: 0, text: null)
  {
  }

  public Languages(IEnumerable<Language> languages, int extra, string? text)
    : this(languages.Select(language => language.Id), extra, text)
  {
  }

  public Languages(IEnumerable<LanguageId> ids, int extra, string? text)
    : this(ids.ToArray(), extra, text)
  {
  }

  [JsonConstructor]
  public Languages(IReadOnlyCollection<LanguageId> ids, int extra, string? text)
  {
    Ids = ids.Distinct().ToArray().AsReadOnly();
    Extra = extra;
    Text = text?.CleanTrim();
    new LanguagesValidator().ValidateAndThrow(this);
  }
}
