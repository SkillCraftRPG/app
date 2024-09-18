using GraphQL.Types;
using SkillCraft.Contracts.Customizations;
using SkillCraft.GraphQL.Search;

namespace SkillCraft.GraphQL.Customizations;

internal class CustomizationSortOptionGraphType : SortOptionInputGraphType<CustomizationSortOption>
{
  public CustomizationSortOptionGraphType() : base()
  {
    Field(x => x.Field, type: typeof(NonNullGraphType<CustomizationSortGraphType>))
      .Description("The field on which to apply the sort.");
  }
}
