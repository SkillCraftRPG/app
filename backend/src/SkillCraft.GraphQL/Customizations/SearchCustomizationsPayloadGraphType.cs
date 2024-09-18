using GraphQL.Types;
using SkillCraft.Contracts.Customizations;
using SkillCraft.GraphQL.Search;

namespace SkillCraft.GraphQL.Customizations;

internal class SearchCustomizationsPayloadGraphType : SearchPayloadInputGraphType<SearchCustomizationsPayload>
{
  public SearchCustomizationsPayloadGraphType() : base()
  {
    Field(x => x.Type, type: typeof(CustomizationTypeGraphType))
      .Description("The type of the customizations to match.");

    Field(x => x.Sort, type: typeof(NonNullGraphType<ListGraphType<NonNullGraphType<CustomizationSortOptionGraphType>>>))
      .DefaultValue([])
      .Description("The sort parameters of the search.");
  }
}
