using Logitar.Portal.Contracts.Search;
using Microsoft.AspNetCore.Mvc;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Talents;
using SkillCraft.Models.Search;

namespace SkillCraft.Models.Talents;

public record SearchTalentsParameters : SearchParameters
{
  [FromQuery(Name = "multiple")]
  public bool? AllowMultiplePurchases { get; set; }

  [FromQuery(Name = "skill")]
  public bool? HasSkill { get; set; }

  [FromQuery(Name = "tier_operator")]
  public string? TierOperator { get; set; }

  [FromQuery(Name = "tiers")]
  public IEnumerable<int>? TierValues { get; set; }

  public SearchTalentsPayload ToPayload()
  {
    SearchTalentsPayload payload = new()
    {
      AllowMultiplePurchases = AllowMultiplePurchases,
      HasSkill = HasSkill
    };
    if (TierValues != null)
    {
      payload.Tier = new TierFilter(TierOperator ?? string.Empty, TierValues);
    }
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
