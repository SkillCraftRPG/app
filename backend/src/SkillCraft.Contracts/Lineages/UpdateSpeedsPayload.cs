namespace SkillCraft.Contracts.Lineages;

public record UpdateSpeedsPayload
{
  public int? Walk { get; set; }
  public int? Climb { get; set; }
  public int? Swim { get; set; }
  public int? Fly { get; set; }
  public int? Hover { get; set; }
  public int? Burrow { get; set; }
}
