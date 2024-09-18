using GraphQL.Types;
using SkillCraft.Contracts.Castes;
using SkillCraft.GraphQL.Search;

namespace SkillCraft.GraphQL.Castes;

internal class CasteSortOptionGraphType : SortOptionInputGraphType<CasteSortOption>
{
  public CasteSortOptionGraphType() : base()
  {
    Field(x => x.Field, type: typeof(NonNullGraphType<CasteSortGraphType>))
      .Description("The field on which to apply the sort.");
  }
}
