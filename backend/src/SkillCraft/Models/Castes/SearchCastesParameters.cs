using Logitar.Portal.Contracts.Search;
using Microsoft.AspNetCore.Mvc;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Castes;
using SkillCraft.Models.Search;

namespace SkillCraft.Models.Castes;

public record SearchCastesParameters : SearchParameters
{
  [FromQuery(Name = "skill")]
  public Skill? Skill { get; set; }

  public SearchCastesPayload ToPayload()
  {
    SearchCastesPayload payload = new()
    {
      Skill = Skill
    };
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
