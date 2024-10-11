using GraphQL.Types;
using SkillCraft.Contracts.Lineages;
using SkillCraft.GraphQL.Search;

namespace SkillCraft.GraphQL.Lineages;

internal class SearchLineagesPayloadGraphType : SearchPayloadInputGraphType<SearchLineagesPayload>
{
  public SearchLineagesPayloadGraphType() : base()
  {
    Field(x => x.Attribute, type: typeof(AttributeGraphType))
      .Description("When specified, only lineages granting a bonus to this attribute will match.");
    Field(x => x.LanguageId)
      .Description("When specified, only lineages learning this language will match.");
    Field(x => x.ParentId)
      .Description("When specified, only nations of this species will match.");
    Field(x => x.SizeCategory, type: typeof(SizeCategoryGraphType))
      .Description("When specified, only lineages in this size category will match.");

    Field(x => x.Sort, type: typeof(NonNullGraphType<ListGraphType<NonNullGraphType<LineageSortOptionGraphType>>>))
      .DefaultValue([])
      .Description("The sort parameters of the search.");
  }
}
