namespace SkillCraft.Contracts.Worlds;

public record CreateWorldPayload
{
  public string Slug { get; set; }
  public string? Name { get; set; }
  public string? Description { get; set; }

  public CreateWorldPayload() : this(string.Empty)
  {
  }

  public CreateWorldPayload(string slug)
  {
    Slug = slug;
  }
}
