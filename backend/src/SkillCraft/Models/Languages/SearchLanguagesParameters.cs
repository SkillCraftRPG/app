using Logitar.Portal.Contracts.Search;
using SkillCraft.Contracts.Languages;
using SkillCraft.Models.Search;

namespace SkillCraft.Models.Languages;

public record SearchLanguagesParameters : SearchParameters
{
  public SearchLanguagesPayload ToPayload()
  {
    SearchLanguagesPayload payload = new();
    Fill(payload);

    foreach (SortOption sort in ((SearchPayload)payload).Sort)
    {
      if (Enum.TryParse(sort.Field, out LanguageSort field))
      {
        payload.Sort.Add(new LanguageSortOption(field, sort.IsDescending));
      }
    }

    return payload;
  }
}
