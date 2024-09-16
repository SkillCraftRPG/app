using Logitar.Portal.Contracts.Search;

namespace SkillCraft.Contracts.Languages;

public record LanguageSortOption : SortOption
{
  public new LanguageSort Field
  {
    get => Enum.Parse<LanguageSort>(base.Field);
    set => base.Field = value.ToString();
  }

  public LanguageSortOption(LanguageSort field, bool isDescending = false) : base(field.ToString(), isDescending)
  {
  }
}
