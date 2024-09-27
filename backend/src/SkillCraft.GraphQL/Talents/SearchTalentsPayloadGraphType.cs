using GraphQL.Types;
using SkillCraft.Contracts.Talents;
using SkillCraft.GraphQL.Search;

namespace SkillCraft.GraphQL.Talents;

internal class SearchTalentsPayloadGraphType : SearchPayloadInputGraphType<SearchTalentsPayload>
{
  public SearchTalentsPayloadGraphType() : base()
  {
    Field(x => x.AllowMultiplePurchases)
      .Description("When specified, only talents that allow or do not allow multiple purchases will match.");
    Field(x => x.HasSkill)
      .Description("When specified, only talents associated to a skill or not will match.");
    Field(x => x.Tier, type: typeof(TierFilterGraphType))
      .Description("When specified, only talents matching the filter will be returned.");

    Field(x => x.Sort, type: typeof(NonNullGraphType<ListGraphType<NonNullGraphType<TalentSortOptionGraphType>>>))
      .DefaultValue([])
      .Description("The sort parameters of the search.");
  }
}
