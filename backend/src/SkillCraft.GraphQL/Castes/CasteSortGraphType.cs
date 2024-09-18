using GraphQL.Types;
using SkillCraft.Contracts.Castes;

namespace SkillCraft.GraphQL.Castes;

internal class CasteSortGraphType : EnumerationGraphType<CasteSort>
{
  public CasteSortGraphType()
  {
    Name = nameof(CasteSort);
    Description = "Represents the available caste fields for sorting.";

    AddValue(CasteSort.Name, "The castes will be sorted by their display name.");
    AddValue(CasteSort.UpdatedOn, "The castes will be sorted by their latest update date and time.");
  }
  private void AddValue(CasteSort value, string description)
  {
    Add(value.ToString(), value, description);
  }
}
