namespace SkillCraft.Contracts.Worlds;

public record ReplaceWorldPayload
{
  public string UniqueSlug { get; set; }
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public ReplaceWorldPayload() : this(string.Empty)
  {
  }

  public ReplaceWorldPayload(string uniqueSlug)
  {
    UniqueSlug = uniqueSlug;
  }
}
