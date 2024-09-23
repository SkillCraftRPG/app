using Logitar.Portal.Contracts.Search;
using SkillCraft.Contracts.Parties;
using SkillCraft.Models.Search;

namespace SkillCraft.Models.Parties;

public record SearchPartiesParameters : SearchParameters
{
  public SearchPartiesPayload ToPayload()
  {
    SearchPartiesPayload payload = new();
    Fill(payload);

    foreach (SortOption sort in ((SearchPayload)payload).Sort)
    {
      if (Enum.TryParse(sort.Field, out PartySort field))
      {
        payload.Sort.Add(new PartySortOption(field, sort.IsDescending));
      }
    }

    return payload;
  }
}
