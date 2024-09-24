using SkillCraft.Contracts.Comments;
using SkillCraft.GraphQL.Search;

namespace SkillCraft.GraphQL.Comments;

internal class CommentSearchResultsGraphType : SearchResultsGraphType<CommentGraphType, CommentModel>
{
  public CommentSearchResultsGraphType() : base("CommentSearchResults", "Represents the results of a comment search.")
  {
  }
}
