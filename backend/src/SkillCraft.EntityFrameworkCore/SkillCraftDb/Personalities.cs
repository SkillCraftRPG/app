using Logitar.Data;
using SkillCraft.EntityFrameworkCore.Entities;

namespace SkillCraft.EntityFrameworkCore.SkillCraftDb;

internal static class Personalities
{
  public static readonly TableId Table = new(nameof(SkillCraftContext.Personalities));

  public static readonly ColumnId AggregateId = new(nameof(PersonalityEntity.AggregateId), Table);
  public static readonly ColumnId CreatedBy = new(nameof(PersonalityEntity.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(PersonalityEntity.CreatedOn), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(PersonalityEntity.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(PersonalityEntity.UpdatedOn), Table);
  public static readonly ColumnId Version = new(nameof(PersonalityEntity.Version), Table);

  public static readonly ColumnId Attribute = new(nameof(PersonalityEntity.Attribute), Table);
  public static readonly ColumnId Description = new(nameof(PersonalityEntity.Description), Table);
  public static readonly ColumnId GiftId = new(nameof(PersonalityEntity.GiftId), Table);
  public static readonly ColumnId Id = new(nameof(PersonalityEntity.Id), Table);
  public static readonly ColumnId Name = new(nameof(PersonalityEntity.Name), Table);
  public static readonly ColumnId PersonalityId = new(nameof(PersonalityEntity.PersonalityId), Table);
  public static readonly ColumnId WorldId = new(nameof(PersonalityEntity.WorldId), Table);
}
