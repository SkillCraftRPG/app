using GraphQL.Types;
using SkillCraft.Contracts.Personalities;
using SkillCraft.GraphQL.Search;

namespace SkillCraft.GraphQL.Personalities;

internal class SearchPersonalitiesPayloadGraphType : SearchPayloadInputGraphType<SearchPersonalitiesPayload>
{
  public SearchPersonalitiesPayloadGraphType() : base()
  {
    Field(x => x.Attribute, type: typeof(AttributeGraphType))
      .Description("When specified, only personalities granting a bonus to this attribute will match.");
    Field(x => x.GiftId, type: typeof(IdGraphType))
      .Description("When specified, only personalities granting this gift will match.");

    Field(x => x.Sort, type: typeof(NonNullGraphType<ListGraphType<NonNullGraphType<PersonalitySortOptionGraphType>>>))
      .DefaultValue([])
      .Description("The sort parameters of the search.");
  }
}
