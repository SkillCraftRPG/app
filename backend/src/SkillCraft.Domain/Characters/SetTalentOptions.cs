namespace SkillCraft.Domain.Characters;

public record SetTalentOptions
{
  public int? Cost { get; set; }
  public Name? Precision { get; set; }
  public Description? Notes { get; set; }
}
