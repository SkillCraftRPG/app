namespace SkillCraft.Contracts.Comments;

public record PostCommentPayload
{
  public string Text { get; set; }

  public PostCommentPayload() : this(string.Empty)
  {
  }

  public PostCommentPayload(string text)
  {
    Text = text;
  }
}
