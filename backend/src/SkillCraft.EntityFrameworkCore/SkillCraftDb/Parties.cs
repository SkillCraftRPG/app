using Logitar.Data;
using SkillCraft.EntityFrameworkCore.Entities;

namespace SkillCraft.EntityFrameworkCore.SkillCraftDb;

internal static class Parties
{
  public static readonly TableId Table = new(nameof(SkillCraftContext.Parties));

  public static readonly ColumnId AggregateId = new(nameof(PartyEntity.AggregateId), Table);
  public static readonly ColumnId CreatedBy = new(nameof(PartyEntity.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(PartyEntity.CreatedOn), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(PartyEntity.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(PartyEntity.UpdatedOn), Table);
  public static readonly ColumnId Version = new(nameof(PartyEntity.Version), Table);

  public static readonly ColumnId Description = new(nameof(PartyEntity.Description), Table);
  public static readonly ColumnId Id = new(nameof(PartyEntity.Id), Table);
  public static readonly ColumnId Name = new(nameof(PartyEntity.Name), Table);
  public static readonly ColumnId PartyId = new(nameof(PartyEntity.PartyId), Table);
  public static readonly ColumnId WorldId = new(nameof(PartyEntity.WorldId), Table);
}
