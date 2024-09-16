using Logitar.Portal.Contracts.Search;
using SkillCraft.Contracts.Personalities;
using SkillCraft.Models.Search;

namespace SkillCraft.Models.Personalities;

public record SearchPersonalitiesParameters : SearchParameters
{
  public SearchPersonalitiesPayload ToPayload()
  {
    SearchPersonalitiesPayload payload = new();
    Fill(payload);

    foreach (SortOption sort in ((SearchPayload)payload).Sort)
    {
      if (Enum.TryParse(sort.Field, out PersonalitySort field))
      {
        payload.Sort.Add(new PersonalitySortOption(field, sort.IsDescending));
      }
    }

    return payload;
  }
}
