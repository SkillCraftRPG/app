using Logitar.Portal.Contracts.Search;
using Microsoft.AspNetCore.Mvc;
using SkillCraft.Contracts.Lineages;
using SkillCraft.Models.Search;

namespace SkillCraft.Models.Lineages;

public record SearchLineagesParameters : SearchParameters
{
  [FromQuery(Name = "parent")]
  public Guid? ParentId { get; set; }

  public SearchLineagesPayload ToPayload()
  {
    SearchLineagesPayload payload = new()
    {
      ParentId = ParentId
    };
    Fill(payload);

    foreach (SortOption sort in ((SearchPayload)payload).Sort)
    {
      if (Enum.TryParse(sort.Field, out LineageSort field))
      {
        payload.Sort.Add(new LineageSortOption(field, sort.IsDescending));
      }
    }

    return payload;
  }
}
