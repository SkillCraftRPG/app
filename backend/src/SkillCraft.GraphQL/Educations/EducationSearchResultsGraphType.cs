using SkillCraft.Contracts.Educations;
using SkillCraft.GraphQL.Search;

namespace SkillCraft.GraphQL.Educations;

internal class EducationSearchResultsGraphType : SearchResultsGraphType<EducationGraphType, EducationModel>
{
  public EducationSearchResultsGraphType() : base("EducationSearchResults", "Represents the results of an education search.")
  {
  }
}
