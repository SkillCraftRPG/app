using GraphQL.Types;
using SkillCraft.Contracts.Personalities;

namespace SkillCraft.GraphQL.Personalities;

internal class PersonalitySortGraphType : EnumerationGraphType<PersonalitySort>
{
  public PersonalitySortGraphType()
  {
    Name = nameof(PersonalitySort);
    Description = "Represents the available personality fields for sorting.";

    AddValue(PersonalitySort.Name, "The personalities will be sorted by their display name.");
    AddValue(PersonalitySort.UpdatedOn, "The personalities will be sorted by their latest update date and time.");
  }
  private void AddValue(PersonalitySort value, string description)
  {
    Add(value.ToString(), value, description);
  }
}
