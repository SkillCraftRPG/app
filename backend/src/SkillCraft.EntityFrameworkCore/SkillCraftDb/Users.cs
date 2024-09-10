using Logitar.Data;
using SkillCraft.EntityFrameworkCore.Entities;

namespace SkillCraft.EntityFrameworkCore.SkillCraftDb;

internal static class Users
{
  public static readonly TableId Table = new(nameof(SkillCraftContext.Users));

  public static readonly ColumnId DisplayName = new(nameof(UserEntity.DisplayName), Table);
  public static readonly ColumnId EmailAddress = new(nameof(UserEntity.EmailAddress), Table);
  public static readonly ColumnId Id = new(nameof(UserEntity.Id), Table);
  public static readonly ColumnId IsDeleted = new(nameof(UserEntity.IsDeleted), Table);
  public static readonly ColumnId PictureUrl = new(nameof(UserEntity.PictureUrl), Table);
  public static readonly ColumnId UserId = new(nameof(UserEntity.UserId), Table);
}
