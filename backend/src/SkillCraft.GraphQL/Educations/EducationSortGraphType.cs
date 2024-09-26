using GraphQL.Types;
using SkillCraft.Contracts.Educations;

namespace SkillCraft.GraphQL.Educations;

internal class EducationSortGraphType : EnumerationGraphType<EducationSort>
{
  public EducationSortGraphType()
  {
    Name = nameof(EducationSort);
    Description = "Represents the available education fields for sorting.";

    AddValue(EducationSort.CreatedOn, "The educations will be sorted by their latest creation date and time.");
    AddValue(EducationSort.Name, "The educations will be sorted by their display name.");
    AddValue(EducationSort.UpdatedOn, "The educations will be sorted by their latest update date and time.");
  }
  private void AddValue(EducationSort value, string description)
  {
    Add(value.ToString(), value, description);
  }
}
