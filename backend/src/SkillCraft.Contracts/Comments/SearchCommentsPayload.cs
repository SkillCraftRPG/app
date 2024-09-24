namespace SkillCraft.Contracts.Comments;

public record SearchCommentsPayload
{
  public List<CommentSortOption> Sort { get; set; } = [];

  public int Skip { get; set; }
  public int Limit { get; set; }
}
