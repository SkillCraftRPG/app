namespace SkillCraft.Contracts.Comments;

public record CreateCommentPayload
{
  public string Text { get; set; }

  public CreateCommentPayload() : this(string.Empty)
  {
  }

  public CreateCommentPayload(string text)
  {
    Text = text;
  }
}
