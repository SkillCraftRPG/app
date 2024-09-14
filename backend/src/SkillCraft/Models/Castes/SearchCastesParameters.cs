using Logitar.Portal.Contracts.Search;
using SkillCraft.Contracts.Castes;
using SkillCraft.Models.Search;

namespace SkillCraft.Models.Castes;

public record SearchCastesParameters : SearchParameters
{
  public SearchCastesPayload ToPayload()
  {
    SearchCastesPayload payload = new();
    Fill(payload);

    foreach (SortOption sort in ((SearchPayload)payload).Sort)
    {
      if (Enum.TryParse(sort.Field, out CasteSort field))
      {
        payload.Sort.Add(new CasteSortOption(field, sort.IsDescending));
      }
    }

    return payload;
  }
}
