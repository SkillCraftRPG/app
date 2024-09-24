using Logitar.Portal.Contracts.Search;

namespace SkillCraft.Contracts.Comments;

public record CommentSortOption : SortOption
{
  public new CommentSort Field
  {
    get => Enum.Parse<CommentSort>(base.Field);
    set => base.Field = value.ToString();
  }

  public CommentSortOption(CommentSort field, bool isDescending = false) : base(field.ToString(), isDescending)
  {
  }
}
