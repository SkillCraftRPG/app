using Logitar.Portal.Contracts.Search;
using SkillCraft.Contracts.Aspects;
using SkillCraft.Models.Search;

namespace SkillCraft.Models.Aspects;

public record SearchAspectsParameters : SearchParameters
{
  public SearchAspectsPayload ToPayload()
  {
    SearchAspectsPayload payload = new();
    Fill(payload);

    foreach (SortOption sort in ((SearchPayload)payload).Sort)
    {
      if (Enum.TryParse(sort.Field, out AspectSort field))
      {
        payload.Sort.Add(new AspectSortOption(field, sort.IsDescending));
      }
    }

    return payload;
  }
}
