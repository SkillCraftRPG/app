using SkillCraft.Contracts.Comments;
using SkillCraft.Domain.Comments;

namespace SkillCraft.Application.Comments;

public interface ICommentQuerier
{
  Task<CommentModel> ReadAsync(Comment comment, CancellationToken cancellationToken = default);
  Task<CommentModel?> ReadAsync(CommentId id, CancellationToken cancellationToken = default);
  Task<CommentModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
}
