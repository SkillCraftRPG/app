using Logitar.Portal.Contracts.Search;
using SkillCraft.Contracts.Talents;
using SkillCraft.Models.Search;

namespace SkillCraft.Models.Talents;

public record SearchTalentsParameters : SearchParameters
{
  public SearchTalentsPayload ToPayload()
  {
    SearchTalentsPayload payload = new();
    Fill(payload);

    foreach (SortOption sort in ((SearchPayload)payload).Sort)
    {
      if (Enum.TryParse(sort.Field, out TalentSort field))
      {
        payload.Sort.Add(new TalentSortOption(field, sort.IsDescending));
      }
    }

    return payload;
  }
}
