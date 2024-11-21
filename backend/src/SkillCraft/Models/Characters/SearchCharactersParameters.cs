using Logitar.Portal.Contracts.Search;
using Microsoft.AspNetCore.Mvc;
using SkillCraft.Contracts.Characters;
using SkillCraft.Models.Search;

namespace SkillCraft.Models.Characters;

public record SearchCharactersParameters : SearchParameters
{
  [FromQuery(Name = "player")]
  public string? PlayerName { get; set; }

  public SearchCharactersPayload ToPayload()
  {
    SearchCharactersPayload payload = new()
    {
      PlayerName = PlayerName
    };
    Fill(payload);

    foreach (SortOption sort in ((SearchPayload)payload).Sort)
    {
      if (Enum.TryParse(sort.Field, out CharacterSort field))
      {
        payload.Sort.Add(new CharacterSortOption(field, sort.IsDescending));
      }
    }

    return payload;
  }
}
