using GraphQL.Types;
using SkillCraft.Contracts.Talents;

namespace SkillCraft.GraphQL.Talents;

internal class TalentSortGraphType : EnumerationGraphType<TalentSort>
{
  public TalentSortGraphType()
  {
    Name = nameof(TalentSort);
    Description = "Represents the available talent fields for sorting.";

    AddValue(TalentSort.Name, "The talents will be sorted by their display name.");
    AddValue(TalentSort.UpdatedOn, "The talents will be sorted by their latest update date and time.");
  }
  private void AddValue(TalentSort value, string description)
  {
    Add(value.ToString(), value, description);
  }
}
