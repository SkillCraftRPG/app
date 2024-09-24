using GraphQL.Types;
using SkillCraft.Contracts.Comments;
using SkillCraft.GraphQL.Search;

namespace SkillCraft.GraphQL.Comments;

internal class CommentSortOptionGraphType : SortOptionInputGraphType<CommentSortOption>
{
  public CommentSortOptionGraphType() : base()
  {
    Field(x => x.Field, type: typeof(NonNullGraphType<CommentSortGraphType>))
      .Description("The field on which to apply the sort.");
  }
}
