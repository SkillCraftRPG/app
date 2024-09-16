using FluentValidation;
using Logitar;
using SkillCraft.Domain.Languages;
using SkillCraft.Domain.Speciez.Validators;

namespace SkillCraft.Domain.Speciez;

public record Languages
{
  public const int MaximumLength = byte.MaxValue;

  public IReadOnlyCollection<LanguageId> Ids { get; }
  public int Extra { get; }
  public string? Text { get; }

  public Languages()
    : this(ids: [], extra: 0, text: null)
  {
  }

  public Languages(IEnumerable<Language> languages, int extra, string? text)
    : this(languages.Select(l => l.Id), extra, text)
  {
  }

  [JsonConstructor]
  public Languages(IEnumerable<LanguageId> ids, int extra, string? text)
  {
    Ids = ids.Distinct().ToList().AsReadOnly();
    Extra = extra;
    Text = text?.CleanTrim();
    new LanguagesValidator().ValidateAndThrow(this);
  }
}
