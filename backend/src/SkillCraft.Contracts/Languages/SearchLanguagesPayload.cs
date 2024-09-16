using Logitar.Portal.Contracts.Search;

namespace SkillCraft.Contracts.Languages;

public record SearchLanguagesPayload : SearchPayload
{
  public new List<LanguageSortOption> Sort { get; set; } = [];
}
