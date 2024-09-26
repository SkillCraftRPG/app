using Logitar.Portal.Contracts.Search;
using Microsoft.AspNetCore.Mvc;
using SkillCraft.Contracts.Personalities;
using SkillCraft.Models.Search;
using Attribute = SkillCraft.Contracts.Attribute;

namespace SkillCraft.Models.Personalities;

public record SearchPersonalitiesParameters : SearchParameters
{
  [FromQuery(Name = "attribute")]
  public Attribute? Attribute { get; set; }

  [FromQuery(Name = "gift")]
  public Guid? GiftId { get; set; }

  public SearchPersonalitiesPayload ToPayload()
  {
    SearchPersonalitiesPayload payload = new()
    {
      Attribute = Attribute,
      GiftId = GiftId
    };
    Fill(payload);

    foreach (SortOption sort in ((SearchPayload)payload).Sort)
    {
      if (Enum.TryParse(sort.Field, out PersonalitySort field))
      {
        payload.Sort.Add(new PersonalitySortOption(field, sort.IsDescending));
      }
    }

    return payload;
  }
}
