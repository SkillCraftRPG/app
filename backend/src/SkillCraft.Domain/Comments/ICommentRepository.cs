namespace SkillCraft.Domain.Comments;

public interface ICommentRepository
{
  Task<Comment?> LoadAsync(CommentId id, CancellationToken cancellationToken = default);

  Task SaveAsync(Comment comment, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<Comment> comments, CancellationToken cancellationToken = default);
}
