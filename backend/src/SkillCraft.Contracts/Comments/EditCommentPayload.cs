namespace SkillCraft.Contracts.Comments;

public record EditCommentPayload
{
  public string Text { get; set; }

  public EditCommentPayload() : this(string.Empty)
  {
  }

  public EditCommentPayload(string text)
  {
    Text = text;
  }
}
