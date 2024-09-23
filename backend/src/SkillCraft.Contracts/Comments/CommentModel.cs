using Logitar.Portal.Contracts;

namespace SkillCraft.Contracts.Comments;

public class CommentModel : Aggregate
{
  public string Text { get; set; }

  public CommentModel() : this(string.Empty)
  {
  }

  public CommentModel(string text)
  {
    Text = text;
  }
}
