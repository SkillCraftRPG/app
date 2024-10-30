using SkillCraft.Contracts;

namespace SkillCraft.Domain.Characters;

public record SetItemOptions
{
  public Change<Guid?>? ContainingItemId { get; set; }

  public int? Quantity { get; set; }

  public Change<bool>? IsAttuned { get; set; }
  public bool? IsEquipped { get; set; }
  public bool? IsIdentified { get; set; }
  public Change<bool>? IsProficient { get; set; }
  public Change<Skill?>? Skill { get; set; }

  public Change<int?>? RemainingCharges { get; set; }
  public Change<int?>? RemainingResistance { get; set; }

  public Change<Name>? NameOverride { get; set; }
  public Change<Description>? DescriptionOverride { get; set; }
  public Change<double?>? ValueOverride { get; set; }
}
