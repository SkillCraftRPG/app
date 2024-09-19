using GraphQL.Types;
using SkillCraft.Contracts.Talents;
using SkillCraft.GraphQL.Search;

namespace SkillCraft.GraphQL.Talents;

internal class TalentSortOptionGraphType : SortOptionInputGraphType<TalentSortOption>
{
  public TalentSortOptionGraphType() : base()
  {
    Field(x => x.Field, type: typeof(NonNullGraphType<TalentSortGraphType>))
      .Description("The field on which to apply the sort.");
  }
}
