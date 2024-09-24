using GraphQL.Types;
using SkillCraft.Contracts.Comments;

namespace SkillCraft.GraphQL.Comments;

internal class CommentSortGraphType : EnumerationGraphType<CommentSort>
{
  public CommentSortGraphType()
  {
    Name = nameof(CommentSort);
    Description = "Represents the available comment fields for sorting.";

    AddValue(CommentSort.CreatedOn, "The comments will be sorted by creation date and time.");
    AddValue(CommentSort.UpdatedOn, "The comments will be sorted by their latest update date and time.");
  }
  private void AddValue(CommentSort value, string description)
  {
    Add(value.ToString(), value, description);
  }
}
