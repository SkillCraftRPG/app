using GraphQL.Types;
using SkillCraft.Contracts.Educations;
using SkillCraft.GraphQL.Search;

namespace SkillCraft.GraphQL.Educations;

internal class SearchEducationsPayloadGraphType : SearchPayloadInputGraphType<SearchEducationsPayload>
{
  public SearchEducationsPayloadGraphType() : base()
  {
    Field(x => x.Sort, type: typeof(NonNullGraphType<ListGraphType<NonNullGraphType<EducationSortOptionGraphType>>>))
      .DefaultValue([])
      .Description("The sort parameters of the search.");
  }
}
