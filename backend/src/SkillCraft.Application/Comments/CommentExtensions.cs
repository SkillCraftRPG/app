using SkillCraft.Domain;
using SkillCraft.Domain.Comments;

namespace SkillCraft.Application.Comments;

internal static class CommentExtensions
{
  private const EntityType Type = EntityType.Comment;

  public static EntityMetadata GetMetadata(this Comment comment)
  {
    long size = comment.Text.Size; // TODO(fpion): complete
    return new EntityMetadata(comment.WorldId, new EntityKey(Type, comment.Id.ToGuid()), size);
  }
}
