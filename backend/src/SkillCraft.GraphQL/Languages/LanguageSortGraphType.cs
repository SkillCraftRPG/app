using GraphQL.Types;
using SkillCraft.Contracts.Languages;

namespace SkillCraft.GraphQL.Languages;

internal class LanguageSortGraphType : EnumerationGraphType<LanguageSort>
{
  public LanguageSortGraphType()
  {
    Name = nameof(LanguageSort);
    Description = "Represents the available language fields for sorting.";

    AddValue(LanguageSort.Name, "The languages will be sorted by their display name.");
    AddValue(LanguageSort.UpdatedOn, "The languages will be sorted by their latest update date and time.");
  }
  private void AddValue(LanguageSort value, string description)
  {
    Add(value.ToString(), value, description);
  }
}
