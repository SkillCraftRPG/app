using SkillCraft.Contracts.Parties;
using SkillCraft.GraphQL.Search;

namespace SkillCraft.GraphQL.Parties;

internal class PartySearchResultsGraphType : SearchResultsGraphType<PartyGraphType, PartyModel>
{
  public PartySearchResultsGraphType() : base("PartySearchResults", "Represents the results of a party search.")
  {
  }
}
