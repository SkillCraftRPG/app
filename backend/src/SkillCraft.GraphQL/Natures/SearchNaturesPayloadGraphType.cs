using GraphQL.Types;
using SkillCraft.Contracts.Natures;
using SkillCraft.GraphQL.Search;

namespace SkillCraft.GraphQL.Natures;

internal class SearchNaturesPayloadGraphType : SearchPayloadInputGraphType<SearchNaturesPayload>
{
  public SearchNaturesPayloadGraphType() : base()
  {
    Field(x => x.Attribute, type: typeof(AttributeGraphType))
      .Description("When specified, only natures granting a bonus to this attribute will match.");
    Field(x => x.GiftId, type: typeof(IdGraphType))
      .Description("When specified, only natures granting this gift will match.");

    Field(x => x.Sort, type: typeof(NonNullGraphType<ListGraphType<NonNullGraphType<NatureSortOptionGraphType>>>))
      .DefaultValue([])
      .Description("The sort parameters of the search.");
  }
}
