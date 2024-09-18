namespace SkillCraft.Contracts.Lineages;

public record UpdateSizePayload
{
  public SizeCategory? Category { get; set; }
  public Change<string>? Roll { get; set; }
}
