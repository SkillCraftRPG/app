using Logitar.Portal.Contracts.Search;
using SkillCraft.Contracts.Educations;
using SkillCraft.Models.Search;

namespace SkillCraft.Models.Educations;

public record SearchEducationsParameters : SearchParameters
{
  public SearchEducationsPayload ToPayload()
  {
    SearchEducationsPayload payload = new();
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
