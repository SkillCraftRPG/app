using SkillCraft.Contracts.Personalities;
using SkillCraft.GraphQL.Search;

namespace SkillCraft.GraphQL.Personalities;

internal class PersonalitySearchResultsGraphType : SearchResultsGraphType<PersonalityGraphType, PersonalityModel>
{
  public PersonalitySearchResultsGraphType() : base("PersonalitySearchResults", "Represents the results of a personality search.")
  {
  }
}
