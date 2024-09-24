using GraphQL.Types;
using SkillCraft.Contracts.Comments;

namespace SkillCraft.GraphQL.Comments;

internal class SearchCommentsPayloadGraphType : InputObjectGraphType<SearchCommentsPayload>
{
  public SearchCommentsPayloadGraphType() : base()
  {
    Name = nameof(SearchCommentsPayload);
    Description = "Represents the parameters of a search.";

    Field(x => x.Sort, type: typeof(NonNullGraphType<ListGraphType<NonNullGraphType<CommentSortOptionGraphType>>>))
      .DefaultValue([])
      .Description("The sort parameters of the search.");

    Field(x => x.Skip).DefaultValue(0)
      .Description("The minimum number of results to skip.");
    Field(x => x.Limit).DefaultValue(0)
      .Description("The maximum number of results to return.");
  }
}
