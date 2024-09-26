using Logitar.Portal.Contracts.Search;
using Microsoft.AspNetCore.Mvc;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Aspects;
using SkillCraft.Models.Search;
using Attribute = SkillCraft.Contracts.Attribute;

namespace SkillCraft.Models.Aspects;

public record SearchAspectsParameters : SearchParameters
{
  [FromQuery(Name = "attribute")]
  public Attribute? Attribute { get; set; }

  [FromQuery(Name = "skill")]
  public Skill? Skill { get; set; }

  public SearchAspectsPayload ToPayload()
  {
    SearchAspectsPayload payload = new()
    {
      Attribute = Attribute,
      Skill = Skill
    };
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
