using Logitar.Portal.Contracts.Search;
using SkillCraft.Contracts.Worlds;
using SkillCraft.Models.Search;

namespace SkillCraft.Models.Worlds;

public record SearchWorldsParameters : SearchParameters
{
  public SearchWorldsPayload ToPayload()
  {
    SearchWorldsPayload payload = new();
    Fill(payload);

    foreach (SortOption sort in ((SearchPayload)payload).Sort)
    {
      if (Enum.TryParse(sort.Field, out WorldSort field))
      {
        payload.Sort.Add(new WorldSortOption(field, sort.IsDescending));
      }
    }

    return payload;
  }
}
