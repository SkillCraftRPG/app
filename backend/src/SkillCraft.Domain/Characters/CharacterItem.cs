using SkillCraft.Contracts;
using SkillCraft.Domain.Items;

namespace SkillCraft.Domain.Characters;

public record CharacterItem(
  ItemId ItemId,
  Guid? ContainingItemId,
  int Quantity,
  bool? IsAttuned,
  bool IsEquipped,
  bool IsIdentified,
  bool? IsProficient,
  Skill? Skill,
  int? RemainingCharges,
  int? RemainingResistance,
  Name? NameOverride,
  Description? DescriptionOverride,
  double? ValueOverride);
