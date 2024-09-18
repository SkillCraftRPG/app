using GraphQL.Types;
using SkillCraft.Contracts.Educations;
using SkillCraft.GraphQL.Search;

namespace SkillCraft.GraphQL.Educations;

internal class EducationSortOptionGraphType : SortOptionInputGraphType<EducationSortOption>
{
  public EducationSortOptionGraphType() : base()
  {
    Field(x => x.Field, type: typeof(NonNullGraphType<EducationSortGraphType>))
      .Description("The field on which to apply the sort.");
  }
}
