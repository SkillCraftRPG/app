namespace SkillCraft.Contracts.Worlds;

public record CreateOrReplaceWorldPayload
{
  public string Slug { get; set; }
  public string? Name { get; set; }
  public string? Description { get; set; }

  public CreateOrReplaceWorldPayload() : this(string.Empty)
  {
  }

  public CreateOrReplaceWorldPayload(string slug)
  {
    Slug = slug;
  }
}
