using SkillCraft.Contracts;
using SkillCraft.Domain;
using SkillCraft.Domain.Comments;

namespace SkillCraft.Application.Comments;

internal static class CommentExtensions
{
  public static EntityMetadata GetMetadata(this Comment comment) => new(comment.WorldId, new EntityKey(EntityType.Comment, comment.Id.ToGuid()), comment.CalculateSize());

  private static long CalculateSize(this Comment comment) => 4 /* EntityType */ + 16 /* EntityId */ + comment.Text.Size;
}
