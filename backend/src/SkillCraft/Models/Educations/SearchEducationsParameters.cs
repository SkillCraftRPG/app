using Logitar.Portal.Contracts.Search;
using Microsoft.AspNetCore.Mvc;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Educations;
using SkillCraft.Models.Search;

namespace SkillCraft.Models.Educations;

public record SearchEducationsParameters : SearchParameters
{
  [FromQuery(Name = "skill")]
  public Skill? Skill { get; set; }

  public SearchEducationsPayload ToPayload()
  {
    SearchEducationsPayload payload = new()
    {
      Skill = Skill
    };
    Fill(payload);

    foreach (SortOption sort in ((SearchPayload)payload).Sort)
    {
      if (Enum.TryParse(sort.Field, out EducationSort field))
      {
        payload.Sort.Add(new EducationSortOption(field, sort.IsDescending));
      }
    }

    return payload;
  }
}
