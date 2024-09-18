namespace SkillCraft.Contracts.Lineages;

public record UpdateSizePayload
{
  public SizeCategory? Category { get; set; }
  public string? Roll { get; set; }
}
