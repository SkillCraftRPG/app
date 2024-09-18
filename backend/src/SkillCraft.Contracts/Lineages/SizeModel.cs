namespace SkillCraft.Contracts.Lineages;

public record SizeModel
{
  public SizeCategory Category { get; set; }
  public string? Roll { get; set; }
}
