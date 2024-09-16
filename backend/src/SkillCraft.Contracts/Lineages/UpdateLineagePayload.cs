namespace SkillCraft.Contracts.Lineages;

public record UpdateLineagePayload
{
  public string? Name { get; set; }
  public Change<string>? Description { get; set; }
}
