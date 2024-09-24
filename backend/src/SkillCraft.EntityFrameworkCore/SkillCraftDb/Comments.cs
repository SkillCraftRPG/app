using Logitar.Data;
using SkillCraft.EntityFrameworkCore.Entities;

namespace SkillCraft.EntityFrameworkCore.SkillCraftDb;

internal static class Comments
{
  public static readonly TableId Table = new(nameof(SkillCraftContext.Comments));

  public static readonly ColumnId AggregateId = new(nameof(CommentEntity.AggregateId), Table);
  public static readonly ColumnId CreatedBy = new(nameof(CommentEntity.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(CommentEntity.CreatedOn), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(CommentEntity.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(CommentEntity.UpdatedOn), Table);
  public static readonly ColumnId Version = new(nameof(CommentEntity.Version), Table);

  public static readonly ColumnId CommentId = new(nameof(CommentEntity.CommentId), Table);
  public static readonly ColumnId EntityId = new(nameof(CommentEntity.EntityId), Table);
  public static readonly ColumnId EntityType = new(nameof(CommentEntity.EntityType), Table);
  public static readonly ColumnId Id = new(nameof(CommentEntity.Id), Table);
  public static readonly ColumnId Text = new(nameof(CommentEntity.Text), Table);
  public static readonly ColumnId WorldId = new(nameof(CommentEntity.WorldId), Table);
}
