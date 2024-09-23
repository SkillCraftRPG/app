using GraphQL.Types;
using SkillCraft.Contracts.Parties;
using SkillCraft.GraphQL.Search;

namespace SkillCraft.GraphQL.Parties;

internal class PartySortOptionGraphType : SortOptionInputGraphType<PartySortOption>
{
  public PartySortOptionGraphType() : base()
  {
    Field(x => x.Field, type: typeof(NonNullGraphType<PartySortGraphType>))
      .Description("The field on which to apply the sort.");
  }
}
