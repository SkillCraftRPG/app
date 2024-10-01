namespace SkillCraft.Contracts.Worlds;

public record SaveWorldPayload
{
  public string Slug { get; set; }
  public string? Name { get; set; }
  public string? Description { get; set; }

  public SaveWorldPayload() : this(string.Empty)
  {
  }

  public SaveWorldPayload(string slug)
  {
    Slug = slug;
  }
}
