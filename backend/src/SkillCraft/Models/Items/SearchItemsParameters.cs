using Logitar.Portal.Contracts.Search;
using Microsoft.AspNetCore.Mvc;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Items;
using SkillCraft.Models.Search;

namespace SkillCraft.Models.Items;

public record SearchItemsParameters : SearchParameters
{
  [FromQuery(Name = "category")]
  public ItemCategory? Category { get; set; }

  [FromQuery(Name = "value_operator")]
  public string? ValueOperator { get; set; }

  [FromQuery(Name = "values")]
  public IEnumerable<double>? ValueValues { get; set; }

  [FromQuery(Name = "weight_operator")]
  public string? WeightOperator { get; set; }

  [FromQuery(Name = "weights")]
  public IEnumerable<double>? WeightValues { get; set; }

  public SearchItemsPayload ToPayload()
  {
    SearchItemsPayload payload = new()
    {
      Category = Category
    };
    if (ValueValues != null)
    {
      payload.Value = new DoubleFilter(ValueOperator ?? string.Empty, ValueValues);
    }
    if (WeightValues != null)
    {
      payload.Weight = new DoubleFilter(WeightOperator ?? string.Empty, WeightValues);
    }
    Fill(payload);

    foreach (SortOption sort in ((SearchPayload)payload).Sort)
    {
      if (Enum.TryParse(sort.Field, out ItemSort field))
      {
        payload.Sort.Add(new ItemSortOption(field, sort.IsDescending));
      }
    }

    return payload;
  }
}
