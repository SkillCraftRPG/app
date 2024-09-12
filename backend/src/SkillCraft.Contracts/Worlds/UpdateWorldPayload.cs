namespace SkillCraft.Contracts.Worlds;

public record UpdateWorldPayload
{
  public string? Slug { get; set; }
  public Change<string>? Name { get; set; }
  public Change<string>? Description { get; set; }
}
