namespace SkillCraft.Contracts.Worlds;

public record ReplaceWorldPayload
{
  public string Slug { get; set; }
  public string? Name { get; set; }
  public string? Description { get; set; }

  public ReplaceWorldPayload() : this(string.Empty)
  {
  }

  public ReplaceWorldPayload(string slug)
  {
    Slug = slug;
  }
}
