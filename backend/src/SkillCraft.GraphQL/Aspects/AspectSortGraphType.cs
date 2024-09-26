using GraphQL.Types;
using SkillCraft.Contracts.Aspects;

namespace SkillCraft.GraphQL.Aspects;

internal class AspectSortGraphType : EnumerationGraphType<AspectSort>
{
  public AspectSortGraphType()
  {
    Name = nameof(AspectSort);
    Description = "Represents the available aspect fields for sorting.";

    AddValue(AspectSort.CreatedOn, "The aspects will be sorted by their creation date and time.");
    AddValue(AspectSort.Name, "The aspects will be sorted by their display name.");
    AddValue(AspectSort.UpdatedOn, "The aspects will be sorted by their latest update date and time.");
  }
  private void AddValue(AspectSort value, string description)
  {
    Add(value.ToString(), value, description);
  }
}
