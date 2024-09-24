using Microsoft.AspNetCore.Mvc;
using SkillCraft.Contracts.Comments;
using SkillCraft.Models.Search;

namespace SkillCraft.Models.Comments;

public record SearchCommentsParameters
{
  [FromQuery(Name = "sort")]
  public IEnumerable<string>? Sort { get; set; }

  [FromQuery(Name = "skip")]
  public int? Skip { get; set; }

  [FromQuery(Name = "limit")]
  public int? Limit { get; set; }

  public SearchCommentsPayload ToPayload()
  {
    SearchCommentsPayload payload = new()
    {
      Skip = Skip ?? 0,
      Limit = Limit ?? 0
    };

    if (Sort != null)
    {
      foreach (string sort in Sort)
      {
        int index = sort.IndexOf(SearchParameters.SortSeparator);
        if (index < 0)
        {
          if (Enum.TryParse(sort, out CommentSort field))
          {
            payload.Sort.Add(new CommentSortOption(field));
          }
        }
        else
        {
          if (Enum.TryParse(sort[(index + 1)..], out CommentSort field))
          {
            bool isDescending = sort[..index].Equals(SearchParameters.DescendingKeyword, StringComparison.InvariantCultureIgnoreCase);
            payload.Sort.Add(new CommentSortOption(field, isDescending));
          }
        }
      }
    }

    return payload;
  }
}
