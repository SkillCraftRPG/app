using SkillCraft.Contracts.Talents;
using SkillCraft.GraphQL.Search;

namespace SkillCraft.GraphQL.Talents;

internal class TalentSearchResultsGraphType : SearchResultsGraphType<TalentGraphType, TalentModel>
{
  public TalentSearchResultsGraphType() : base("TalentSearchResults", "Represents the results of a talent search.")
  {
  }
}
