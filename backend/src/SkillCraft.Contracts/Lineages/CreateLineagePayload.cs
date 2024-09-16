namespace SkillCraft.Contracts.Lineages;

public record CreateLineagePayload
{
  public string Name { get; set; }
  public string? Description { get; set; }

  public CreateLineagePayload() : this(string.Empty)
  {
  }

  public CreateLineagePayload(string name)
  {
    Name = name;
  }
}
