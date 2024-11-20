using GraphQL.Types;
using SkillCraft.Contracts.Natures;

namespace SkillCraft.GraphQL.Natures;

internal class NatureSortGraphType : EnumerationGraphType<NatureSort>
{
  public NatureSortGraphType()
  {
    Name = nameof(NatureSort);
    Description = "Represents the available nature fields for sorting.";

    AddValue(NatureSort.CreatedOn, "The natures will be sorted by their creation date and time.");
    AddValue(NatureSort.Name, "The natures will be sorted by their display name.");
    AddValue(NatureSort.UpdatedOn, "The natures will be sorted by their latest update date and time.");
  }
  private void AddValue(NatureSort value, string description)
  {
    Add(value.ToString(), value, description);
  }
}
