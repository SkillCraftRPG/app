using Logitar.Data;
using SkillCraft.EntityFrameworkCore.Entities;

namespace SkillCraft.EntityFrameworkCore.SkillCraftDb;

internal static class Characters
{
  public static readonly TableId Table = new(nameof(SkillCraftContext.Characters));

  public static readonly ColumnId AggregateId = new(nameof(CharacterEntity.AggregateId), Table);
  public static readonly ColumnId CreatedBy = new(nameof(CharacterEntity.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(CharacterEntity.CreatedOn), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(CharacterEntity.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(CharacterEntity.UpdatedOn), Table);
  public static readonly ColumnId Version = new(nameof(CharacterEntity.Version), Table);

  public static readonly ColumnId Age = new(nameof(CharacterEntity.Age), Table);
  public static readonly ColumnId Agility = new(nameof(CharacterEntity.Agility), Table);
  public static readonly ColumnId BestAttribute = new(nameof(CharacterEntity.BestAttribute), Table);
  public static readonly ColumnId BloodAlcoholContent = new(nameof(CharacterEntity.BloodAlcoholContent), Table);
  public static readonly ColumnId CasteId = new(nameof(CharacterEntity.CasteId), Table);
  public static readonly ColumnId CharacterId = new(nameof(CharacterEntity.CharacterId), Table);
  public static readonly ColumnId Coordination = new(nameof(CharacterEntity.Coordination), Table);
  public static readonly ColumnId EducationId = new(nameof(CharacterEntity.EducationId), Table);
  public static readonly ColumnId Experience = new(nameof(CharacterEntity.Experience), Table);
  public static readonly ColumnId ExtraAttributes = new(nameof(CharacterEntity.ExtraAttributes), Table);
  public static readonly ColumnId Height = new(nameof(CharacterEntity.Height), Table);
  public static readonly ColumnId Id = new(nameof(CharacterEntity.Id), Table);
  public static readonly ColumnId Intellect = new(nameof(CharacterEntity.Intellect), Table);
  public static readonly ColumnId Intoxication = new(nameof(CharacterEntity.Intoxication), Table);
  public static readonly ColumnId Level = new(nameof(CharacterEntity.Level), Table);
  public static readonly ColumnId LevelUps = new(nameof(CharacterEntity.LevelUps), Table);
  public static readonly ColumnId LineageId = new(nameof(CharacterEntity.LineageId), Table);
  public static readonly ColumnId MandatoryAttributes = new(nameof(CharacterEntity.MandatoryAttributes), Table);
  public static readonly ColumnId Name = new(nameof(CharacterEntity.Name), Table);
  public static readonly ColumnId NatureId = new(nameof(CharacterEntity.NatureId), Table);
  public static readonly ColumnId OptionalAttributes = new(nameof(CharacterEntity.OptionalAttributes), Table);
  public static readonly ColumnId PlayerName = new(nameof(CharacterEntity.PlayerName), Table);
  public static readonly ColumnId Presence = new(nameof(CharacterEntity.Presence), Table);
  public static readonly ColumnId Sensitivity = new(nameof(CharacterEntity.Sensitivity), Table);
  public static readonly ColumnId Spirit = new(nameof(CharacterEntity.Spirit), Table);
  public static readonly ColumnId Stamina = new(nameof(CharacterEntity.Stamina), Table);
  public static readonly ColumnId Tier = new(nameof(CharacterEntity.Tier), Table);
  public static readonly ColumnId Vigor = new(nameof(CharacterEntity.Vigor), Table);
  public static readonly ColumnId Vitality = new(nameof(CharacterEntity.Vitality), Table);
  public static readonly ColumnId Weight = new(nameof(CharacterEntity.Weight), Table);
  public static readonly ColumnId WorldId = new(nameof(CharacterEntity.WorldId), Table);
  public static readonly ColumnId WorstAttribute = new(nameof(CharacterEntity.WorstAttribute), Table);
}
