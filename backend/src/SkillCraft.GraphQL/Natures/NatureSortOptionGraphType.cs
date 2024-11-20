using GraphQL.Types;
using SkillCraft.Contracts.Natures;
using SkillCraft.GraphQL.Search;

namespace SkillCraft.GraphQL.Natures;

internal class NatureSortOptionGraphType : SortOptionInputGraphType<NatureSortOption>
{
  public NatureSortOptionGraphType() : base()
  {
    Field(x => x.Field, type: typeof(NonNullGraphType<NatureSortGraphType>))
      .Description("The field on which to apply the sort.");
  }
}
