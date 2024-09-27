using GraphQL.Types;
using SkillCraft.Contracts.Lineages;

namespace SkillCraft.GraphQL.Lineages;

internal class LineageSortGraphType : EnumerationGraphType<LineageSort>
{
  public LineageSortGraphType()
  {
    Name = nameof(LineageSort);
    Description = "Represents the available lineage fields for sorting.";

    AddValue(LineageSort.CreatedOn, "The lineages will be sorted by their creation date and time.");
    AddValue(LineageSort.Name, "The lineages will be sorted by their display name.");
    AddValue(LineageSort.UpdatedOn, "The lineages will be sorted by their latest update date and time.");
  }
  private void AddValue(LineageSort value, string description)
  {
    Add(value.ToString(), value, description);
  }
}
