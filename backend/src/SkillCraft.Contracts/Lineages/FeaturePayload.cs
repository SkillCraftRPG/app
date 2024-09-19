namespace SkillCraft.Contracts.Lineages;

public record FeaturePayload
{
  public Guid? Id { get; set; }

  public string Name { get; set; }
  public string? Description { get; set; }

  public FeaturePayload() : this(string.Empty)
  {
  }

  public FeaturePayload(string name)
  {
    Name = name;
  }
}
