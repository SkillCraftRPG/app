using GraphQL.Types;
using SkillCraft.Contracts.Parties;

namespace SkillCraft.GraphQL.Parties;

internal class PartySortGraphType : EnumerationGraphType<PartySort>
{
  public PartySortGraphType()
  {
    Name = nameof(PartySort);
    Description = "Represents the available party fields for sorting.";

    AddValue(PartySort.Name, "The parties will be sorted by their display name.");
    AddValue(PartySort.UpdatedOn, "The parties will be sorted by their latest update date and time.");
  }
  private void AddValue(PartySort value, string description)
  {
    Add(value.ToString(), value, description);
  }
}
