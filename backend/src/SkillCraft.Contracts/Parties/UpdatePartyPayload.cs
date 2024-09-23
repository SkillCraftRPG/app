namespace SkillCraft.Contracts.Parties;

public record UpdatePartyPayload
{
  public string? Name { get; set; }
  public Change<string>? Description { get; set; }
}
