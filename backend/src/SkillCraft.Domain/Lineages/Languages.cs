using FluentValidation;
using Logitar;
using SkillCraft.Domain.Languages;
using SkillCraft.Domain.Lineages.Validators;

namespace SkillCraft.Domain.Lineages;

public record Languages
{
  public const int MaximumLength = byte.MaxValue;

  public IReadOnlySet<LanguageId> Ids { get; }
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
    Ids = ids.Distinct().ToHashSet();
    Extra = extra;
    Text = text?.CleanTrim();
    new LanguagesValidator().ValidateAndThrow(this);
  }
}
