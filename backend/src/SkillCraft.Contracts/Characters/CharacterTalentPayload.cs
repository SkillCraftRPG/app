namespace SkillCraft.Contracts.Characters;

public record CharacterTalentPayload
{
  public Guid TalentId { get; set; }

  public int Cost { get; set; }
  public string? Precision { get; set; }
  public string? Notes { get; set; }
}
